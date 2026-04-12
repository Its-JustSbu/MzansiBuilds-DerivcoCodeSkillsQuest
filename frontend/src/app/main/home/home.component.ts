import { Component, inject, signal } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { MatGridListModule } from '@angular/material/grid-list';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { ProjectCard } from '../../components/project-card/project-card';
import { stageStatus, supportType } from '../../utils/StaticDTOs/lookup';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  imports: [MatGridListModule, ProjectCard],
})
export class HomeComponent {
  private breakpointObserver = inject(BreakpointObserver);
  cards = signal<ProjectViewDTO[]>(mockCards);
  cols = 2;
  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
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
        milestones: [{
          description: 'Milestone 1'
        }],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 3,
        milestones: [],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 4',
        stageNumber: 4,
        milestones: [],
        stageStatus: stageStatus[0]
      }
    ],
    support: [
      {
        description: 'General Support',
        supportType: supportType[2]
      }
    ]
  },
  {
    name: 'Test Project 2',
    description: 'Test Description 2',
    stages: [
      {
        stageTitle: 'Stage 1',
        stageNumber: 1,
        milestones: [{
          description: 'Milestone 1'
        }],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0]
      }
    ],
    support: [
      {
        description: 'General Support',
        supportType: supportType[2]
      }
    ]
  },
  {
    name: 'Test Project 3',
    description: 'Test Description 3',
    stages: [
      {
        stageTitle: 'Stage 1',
        stageNumber: 1,
        milestones: [{
          description: 'Milestone 1'
        }],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0]
      }
    ],
    support: []
  },
  {
    name: 'Test Project 4',
    description: 'Test Description 4',
    stages: [
      {
        stageTitle: 'Stage 1',
        stageNumber: 1,
        milestones: [{
          description: 'Milestone 1'
        }],
        stageStatus: stageStatus[0]
      },
      {
        stageTitle: 'Stage 2',
        stageNumber: 2,
        milestones: [],
        stageStatus: stageStatus[0]
      }
    ],
    support: [
      {
        description: 'General Support',
        supportType: supportType[2]
      }
    ]
  },
];
