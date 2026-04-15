import { Component, inject, OnInit, signal } from '@angular/core';
import {
  CdkDrag,
  CdkDropList,
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ProjectViewDTO, StageViewDTO, SupportViewDTO } from '../../utils/interfaces/ProjectView';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { form, FormField, required } from '@angular/forms/signals';
import { lookup, project, projectStage, support } from '../../utils/interfaces/entities';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';
import { supportType, stageStatus } from '../../utils/StaticDTOs/lookup';
import { AddProjectComponent } from '../add-project/add-project.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-project',
  templateUrl: './update-project.component.html',
  styleUrl: './update-project.component.scss',
  imports: [
    CdkDrag,
    CdkDropList,
    MatButtonModule,
    MatIcon,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    FormField,
    CommonModule,
    FormsModule,
  ],
})
export class UpdateProjectComponent implements OnInit {
  ngOnInit(): void {
    this.apiService.get(`Project/ById/${this.data.id}`).subscribe({
      next: (res: any) => {
        this.project.update(() => res as project);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
    
  }
  data: project = inject(MAT_DIALOG_DATA);
  snackService = inject(Messagebox);
  apiService = inject(Api);
  selectedSupport: lookup = supportType[0];
  selectedStatus: lookup = stageStatus[0];
  stageStatuses: lookup[] = stageStatus;
  supportTypes: lookup[] = supportType;
  project = signal<project>({
    name: '',
    description: '',
    stages: [],
    support: [],
    createdAt: new Date(),
  });
  addedStages = signal<projectStage[]>([]);
  addedSupport = signal<support[]>([]);
  deletedStages = signal<projectStage[]>([]);
  deletedSupport = signal<support[]>([]);

  projectForm = form(this.project, (path) => {
    required(path.name, { message: 'Name is required.' });
    required(path.description, { message: 'description is required.' });
  });

  dialogRef = inject(MatDialogRef<UpdateProjectComponent>);

  drop(event: CdkDragDrop<projectStage[] | undefined>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data as projectStage[],
        event.previousIndex,
        event.currentIndex,
      );

      (event.container.data as projectStage[]).forEach((stage, index) => {
        (event.container.data as projectStage[])[index].stageNumber = index + 1;
      });

      this.project.update((current) => ({
        ...current,
        stages: event.container.data,
      }));
    } else {
      transferArrayItem(
        event.previousContainer.data as any,
        event.container.data as any,
        event.previousIndex,
        event.currentIndex,
      );
    }
  }

  // Update a specific index
  updateItem(index: number, newValue: string) {
    this.project.update((current) => ({
      ...current,
      stages: (current.stages as projectStage[]).map((item, i) =>
        i === index ? newValue : item,
      ) as projectStage[],
    }));
  }

  updateSelection(stage: projectStage, event: lookup) {
    stage.stageStatus = event;
    this.project.update((current) => ({
      ...current,
      stages: [...(current.stages as projectStage[]), stage],
    }));
  }

  addSupport(event: any) {
    const data = new FormData(event);
    console.log(data.get('description'));
    const currenSupport: support = {
      id: 0,
      description: data.get('description') as string,
      supportTypeId: this.selectedSupport.id,
      supportType: undefined,
      requestedAt: new Date(),
    };

    this.addedSupport.update((arr) => [...arr, currenSupport]);

    this.project.update((current) => ({
      ...current,
      support: [...(current.support as support[]), currenSupport],
    }));
  }

  removeSupport(support: support) {
    this.deletedSupport.update((arr) => [...arr, support]);
    this.project.update((current) => ({
      ...current,
      support: (current.support as support[]).filter((m) => m !== support),
    }));
  }

  addStage(event: any) {
    const data = new FormData(event);
    const stageView: projectStage = {
      stageTitle: data.get('title') as string,
      stageNumber: (this.project().stages as projectStage[]).length + 1,
      stageStatusId: this.selectedStatus.id,
      stageStatus: undefined,
      milestones: [],
      modifiedAt: new Date(),
    };

    this.addedStages.update((arr) => [...arr, stageView]);

    this.project.update((current) => ({
      ...current,
      stages: [...(current.stages as projectStage[]), stageView],
    }));
  }

  removeStage(stage: projectStage) {
    this.deletedStages.update((arr) => [...arr, stage]);
    this.project.update((current) => ({
      ...current,
      stages: (current.stages as projectStage[]).filter((m) => m !== stage),
    }));
  }

  onSubmit() {
    if (this.projectForm().invalid()) {
      this.snackService.openWarning('Please complete the form!');
      return;
    }

    if (this.deletedStages().length > 0) {
      this.apiService.delete(`Project/${this.project().id}/stage`, this.deletedStages()).subscribe({
        next: () => {},
        error: (error: any) => {
          this.snackService.openError(error.error.message);
          return;
        },
      });
    }

    if (this.addedStages().length > 0) {
      this.apiService.post(`Project/${this.project().id}/stage`, this.addedStages()).subscribe({
        next: () => {},
        error: (error: any) => {
          this.snackService.openError(error.error.message);
          return;
        },
      });
    }

    if (this.deletedSupport().length > 0) {
      this.deletedSupport().forEach((support) => {
        this.apiService
          .delete(`Project/${support.id}/support`)
          .subscribe({
            next: () => {},
            error: (error: any) => {
              this.snackService.openError(error.error.message);
              return;
            },
          });
      });
    }

    if (this.addedSupport().length > 0) {
      this.addedSupport().forEach((support) => {
        console.log(support);
        this.apiService
          .post(`Project/${this.project().id}/support`, support)
          .subscribe({
            next: () => {},
            error: (error: any) => {
              this.snackService.openError(error.error.message);
              return;
            },
          });
      });
    }

    console.log(this.project())

    this.apiService.put(`Project/${this.project().id}`, this.project()).subscribe({
      next: (res) => {
        this.snackService.openSuccess(res.message);
        this.onClose();
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }

  onClose() {
    this.dialogRef.close();
  }
}
