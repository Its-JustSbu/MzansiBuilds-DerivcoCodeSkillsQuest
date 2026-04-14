import { Component, computed, Input, OnInit, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { milestone, projectStage } from '../../utils/interfaces/entities';

@Component({
  selector: 'app-milestone-list',
  imports: [MatTableModule],
  templateUrl: './milestone-list.html',
  styleUrl: './milestone-list.scss',
})
export class MilestoneList implements OnInit {
  @Input({ required: true }) stages!: projectStage[]

  ngOnInit(): void {
    this.milestonesColumn.update(() => Object.keys({ description: '', modifiedAt: new Date() } as milestone));
    this.stages.forEach((stage) => {
      if (stage)
        this.milestones.update(arr => [...arr, ...stage.milestones as milestone[] ])
    });
    console.log(this.stages);
  }

  milestones = signal<milestone[]>([]);
  milestonesColumn = signal<string[]>([]);
}
