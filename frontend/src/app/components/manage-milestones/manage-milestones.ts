import { Component, inject, OnInit, signal } from '@angular/core';
import { milestone, project, projectStage } from '../../utils/interfaces/entities';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';
import { FormsModule } from '@angular/forms';
import { MilestoneViewDTO } from '../../utils/interfaces/ProjectView';
import { FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-manage-milestones',
  imports: [
    MatFormFieldModule,
    MatIcon,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
  ],
  templateUrl: './manage-milestones.html',
  styleUrl: './manage-milestones.scss',
})
export class ManageMilestones implements OnInit {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  ngOnInit(): void {
    this.apiService.get(`Project/ById/${this.data.id}`).subscribe({
      next: (res: any) => {
        this.stages.update(() => res.stages as projectStage[]);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }

  newMilestone = signal<MilestoneViewDTO[]>([]);
  data: project = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<ManageMilestones>);
  stages = signal<projectStage[]>([]);
  disabled = signal(true);

  onClose() {
    this.dialogRef.close();
  }

  onStageComplete(stage: projectStage) {
    this.stages.update((arr) =>
      arr.map((s) =>
        s.id === stage.id
          ? {
              ...s,
              stageStatus: {
                id: 3,
                name: 'Completed',
              },
            }
          : s,
      ),
    );
    this.disabled.update(() => false);
  }

  onMilestoneChange(event: Event, stageId?: number, milestoneId?: number) {
    const value = (event.target as HTMLInputElement).value;

    this.stages.update((arr) =>
      arr.map((stage) =>
        stage.id === stageId
          ? {
              ...stage,
              milestones: (stage.milestones as milestone[]).map((m) =>
                m.id === milestoneId ? { ...m, description: value } : m,
              ),
            }
          : stage,
      ),
    );
    this.disabled.update(() => false);
  }

  addMilestone(event: any, stage: projectStage) {
    const data = new FormData(event);
    const newMilestone: MilestoneViewDTO = {
      description: data.get('description') as string,
    };
    this.apiService.post(`Project/${stage.id}/milestone`, newMilestone).subscribe({
      next: (res) => {
        this.snackService.openSuccess('Milestone added!');
        this.stages.update((arr) =>
          arr.map((stage) => ({
            ...stage,
            milestones: [...(stage.milestones as milestone[]), res as milestone],
          })),
        );
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }

  onDelete(mileStone: milestone, id?: number) {
    this.apiService.delete(`Project/${mileStone.id}/milestone`).subscribe({
      next: () => {
        this.snackService.openSuccess('Milestone removed! Refresh to see milestone.');
        this.stages.update((arr) =>
          arr.map((stage) => ({
            ...stage,
            milestones: (stage.milestones as milestone[]).filter((m) => m.id !== id),
          })),
        );
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }

  onUpdate() {
    console.log(this.stages());
    this.apiService.put(`Project/${this.data.id}/stage`, this.stages()).subscribe({
      next: (res: any) => {
        this.snackService.openSuccess(res.message);
        this.onClose();
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
