import { Component, computed, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { milestone } from '../../utils/interfaces/entities';

@Component({
  selector: 'app-milestone-list',
  imports: [MatTableModule],
  templateUrl: './milestone-list.html',
  styleUrl: './milestone-list.scss',
})
export class MilestoneList {
  ngOnInit(): void {
    this.milestonesColumn.update(() => Object.keys(this.milestones()[0]));
  }

  milestones = signal<milestone[]>([
    {
      description: 'Milestone',
      modifiedAt: new Date()
    },
  ]);
  milestonesColumn = signal<string[]>([]);
}
