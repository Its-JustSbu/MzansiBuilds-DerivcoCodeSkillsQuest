import { Component, inject, signal } from '@angular/core';
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

@Component({
  selector: 'app-my-projects',
  templateUrl: './my-projects.component.html',
  styleUrl: './my-projects.component.scss',
  imports: [MatGridListModule, ProjectCard, MatIcon, MatAnchor],
})
export class MyProjectsComponent {
  dialog = inject(MatDialog);
  private breakpointObserver = inject(BreakpointObserver);
  cards = signal<ProjectViewDTO[]>(mockCards);
  cols = 2;
  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
  }
  openAdd(){
    this.dialog.open(AddProjectComponent, {
      maxWidth: "95vw",
      width: "100%",
      maxHeight: "660px",
      height: "100%",
    })
  }
}
const mockCards: ProjectViewDTO[] = [
  {
    name: 'Test Project 1',
    description: 'Test Description 1',
    stages: [
      {
        stageTitle: 'Stage 1',
        stageNumber: 1,
        milestones: [
          {
            description: 'Milestone 1',
          },
        ],
        stageStatus: stageStatus[0],
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0],
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 3,
        milestones: [],
        stageStatus: stageStatus[0],
      },
      {
        stageTitle: 'Stage 4',
        stageNumber: 4,
        milestones: [],
        stageStatus: stageStatus[0],
      },
    ],
    support: [
      {
        description: 'General Support',
        supportType: supportType[2],
      },
    ],
  },
  {
    name: 'Test Project 2',
    description: 'Test Description 2',
    stages: [
      {
        stageTitle: 'Stage 1',
        stageNumber: 1,
        milestones: [
          {
            description: 'Milestone 1',
          },
        ],
        stageStatus: stageStatus[0],
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0],
      },
    ],
    support: [
      {
        description: 'General Support',
        supportType: supportType[2],
      },
    ],
  },
];
