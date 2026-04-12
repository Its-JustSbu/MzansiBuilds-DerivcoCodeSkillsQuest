import { Component, inject, OnInit, signal } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';
import { MatGridListModule } from '@angular/material/grid-list';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { CelebrationCard } from '../../components/celebration-card/celebration-card';
import { stageStatus, supportType } from '../../utils/StaticDTOs/lookup';

@Component({
  selector: 'app-celebration-wall',
  templateUrl: './celebration-wall.component.html',
  styleUrl: './celebration-wall.component.scss',
  imports: [MatGridListModule, CelebrationCard],
})
export class CelebrationWallComponent implements OnInit {
  ngOnInit(): void {
    this.cards.update(() => mockCards);
  }
  private breakpointObserver = inject(BreakpointObserver);
  cols = 2;
  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
  }
  cards = signal<ProjectViewDTO[]>([]);
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
];
