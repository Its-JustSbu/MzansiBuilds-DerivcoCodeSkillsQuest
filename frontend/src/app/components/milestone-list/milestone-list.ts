import { Component, computed, Input, OnInit, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { milestone, projectStage } from '../../utils/interfaces/entities';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-milestone-list',
  imports: [MatTableModule, DatePipe],
  templateUrl: './milestone-list.html',
  styleUrl: './milestone-list.scss',
})
export class MilestoneList implements OnInit {
  @Input({ required: true }) stages!: projectStage[]

  ngOnInit(): void {
    this.milestonesColumn.update(() => Object.keys({ description: '', modifiedAt: new Date() } as milestone));
    this.stages.forEach((stage) => {
      if (stage) this.milestones.update(arr => [...arr, ...stage.milestones as milestone[] ])
    });
    console.log(this.stages);
  }

  milestones = signal<milestone[]>([]);
  milestonesColumn = signal<string[]>([]);
  normalColumn(str: string) {
    return str
      .replace(/([A-Z])/g, ' $1') // Adds a space before capital letters [1, 2]
      .replace(/^./, (str: string) => str.toUpperCase()); // Optionally capitalizes the first letter
  }
}
