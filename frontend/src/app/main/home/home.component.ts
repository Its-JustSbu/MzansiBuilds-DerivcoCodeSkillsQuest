import { Component, inject, OnInit, signal } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { MatGridListModule } from '@angular/material/grid-list';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { ProjectCard } from '../../components/project-card/project-card';
import { stageStatus, supportType } from '../../utils/StaticDTOs/lookup';
import { project } from '../../utils/interfaces/entities';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  imports: [MatGridListModule, ProjectCard],
})
export class HomeComponent implements OnInit {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  private breakpointObserver = inject(BreakpointObserver);
  pageNumber = signal(1);
  cards = signal<project[]>([]);
  cols = 2;
  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
  }
  ngOnInit(): void {
    this.getProjects(this.pageNumber());
  }
  getProjects(page: number) {
    this.apiService.get(`Project/${page}`).subscribe({
      next: (res: any) => {
        this.cards.update(() => res as project[]);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
