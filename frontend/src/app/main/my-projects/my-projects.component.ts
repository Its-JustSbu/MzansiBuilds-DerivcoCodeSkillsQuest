import { Component, inject, OnInit, signal } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule, MatIcon } from '@angular/material/icon';
import { MatButtonModule, MatAnchor } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { stageStatus, supportType } from '../../utils/StaticDTOs/lookup';
import { ProjectCard } from '../../components/project-card/project-card';
import { MatDialog } from '@angular/material/dialog';
import { AddProjectComponent } from '../../components/add-project/add-project.component';
import { collaboration, project } from '../../utils/interfaces/entities';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';

@Component({
  selector: 'app-my-projects',
  templateUrl: './my-projects.component.html',
  styleUrl: './my-projects.component.scss',
  imports: [MatGridListModule, ProjectCard, MatIcon, MatAnchor],
})
export class MyProjectsComponent implements OnInit {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  dialog = inject(MatDialog);
  private breakpointObserver = inject(BreakpointObserver);
  cards = signal<collaboration[]>([]);
  cols = 2;
  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
  }
  ngOnInit(): void {
    this.getProjects();
  }
  getProjects() {
    this.apiService.get(`Collaborator`).subscribe({
      next: (res: any) => {
        this.cards.update(() => res as collaboration[]);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
  openAdd() {
    this.dialog.open(AddProjectComponent, {
      maxWidth: '95vw',
      width: '100%',
      maxHeight: '660px',
      height: '100%',
    });
  }
}
