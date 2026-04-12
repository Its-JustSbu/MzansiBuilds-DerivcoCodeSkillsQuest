import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { ProjectViewDTO, StageViewDTO } from '../../utils/interfaces/ProjectView';
import { MatDivider } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-celebration-card',
  imports: [
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatDivider,
    MatListModule,
  ],
  templateUrl: './celebration-card.html',
  styleUrl: './celebration-card.scss',
})
export class CelebrationCard {
  @Input({ required: true }) card!: ProjectViewDTO;
  getNumberOfMilestones(stages?: StageViewDTO[]): number {
    let num = 0;
    if (!stages) return 0;

    stages.forEach((stage) => {
      num += stage.milestones.length;
    });

    return num;
  }
}
