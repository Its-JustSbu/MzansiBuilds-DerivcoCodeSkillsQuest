import { Component, inject, signal } from '@angular/core';
import { projectStage } from '../../utils/interfaces/entities';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-manage-milestones',
  imports: [MatFormFieldModule, MatIcon, MatSelectModule, MatInputModule, MatButtonModule],
  templateUrl: './manage-milestones.html',
  styleUrl: './manage-milestones.scss',
})
export class ManageMilestones {
  dialogRef = inject(MatDialogRef<ManageMilestones>);
  stages = signal<projectStage[]>(mockData);
  onClose(){
    this.dialogRef.close();
  }
}

const mockData: projectStage[] = [
  {
    stageNumber: 1,
    stageTitle: '1st Stage',
    modifiedAt: new Date(),
    milestones: [
      {
        description: 'A milestone',
        modifiedAt: new Date(),
      },
    ],
  },
  {
    stageNumber: 2,
    stageTitle: '2nd Stage',
    modifiedAt: new Date(),
    milestones: [
      {
        description: 'A milestone',
        modifiedAt: new Date(),
      },
    ],
  },
];
