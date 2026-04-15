import { Component, inject, signal } from '@angular/core';
import {
  CdkDrag,
  CdkDropList,
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { MatAnchor, MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ProjectViewDTO, StageViewDTO, SupportViewDTO } from '../../utils/interfaces/ProjectView';
import { form, required, FormField } from '@angular/forms/signals';
import { lookup } from '../../utils/interfaces/entities';
import { stageStatus, supportType } from '../../utils/StaticDTOs/lookup';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';

@Component({
  selector: 'app-add-project',
  templateUrl: './add-project.component.html',
  styleUrl: './add-project.component.scss',
  imports: [
    CdkDrag,
    CdkDropList,
    MatAnchor,
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
export class AddProjectComponent {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  selectedSupport: lookup = supportType[0];
  selectedStatus: lookup = stageStatus[0];
  stageStatuses: lookup[] = stageStatus;
  supportTypes: lookup[] = supportType;
  project = signal<ProjectViewDTO>({
    name: '',
    description: '',
    stages: [],
    support: [],
  });

  projectForm = form(this.project, (path) => {
    required(path.name, { message: 'Name is required.' });
    required(path.description, { message: 'description is required.' });
  });

  dialogRef = inject(MatDialogRef<AddProjectComponent>);

  drop(event: CdkDragDrop<StageViewDTO[] | undefined>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data as StageViewDTO[],
        event.previousIndex,
        event.currentIndex,
      );

      (event.container.data as StageViewDTO[]).forEach((stage, index) => {
        (event.container.data as StageViewDTO[])[index].stageNumber = index + 1;
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
      stages: (current.stages as StageViewDTO[]).map((item, i) =>
        i === index ? newValue : item,
      ) as StageViewDTO[],
    }));
  }

  updateSelection(stage: StageViewDTO, event: lookup) {
    stage.stageStatus = event;
    this.project.update((current) => ({
      ...current,
      stages: [...(current.stages as StageViewDTO[]), stage],
    }));
  }

  addSupport(event: any) {
    const data = new FormData(event);
    console.log(this.selectedSupport);
    const supportView: SupportViewDTO = {
      description: data.get('description') as string,
      supportType: this.selectedSupport as lookup,
    };

    this.project.update((current) => ({
      ...current,
      support: [...(current.support as SupportViewDTO[]), supportView],
    }));
  }

  removeSupport(support: SupportViewDTO) {
    this.project.update((current) => ({
      ...current,
      support: (current.support as SupportViewDTO[]).filter((m) => m !== support),
    }));
  }

  addStage(event: any) {
    const data = new FormData(event);
    const stageView: StageViewDTO = {
      stageTitle: data.get('title') as string,
      stageNumber: (this.project().stages as StageViewDTO[]).length + 1,
      stageStatus: this.selectedStatus,
      milestones: [],
    };

    this.project.update((current) => ({
      ...current,
      stages: [...(current.stages as StageViewDTO[]), stageView],
    }));
  }

  removeStage(stage: StageViewDTO) {
    this.project.update((current) => ({
      ...current,
      stages: (current.stages as StageViewDTO[]).filter((m) => m !== stage),
    }));
  }

  onClose() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.projectForm().invalid()) {
      this.snackService.openWarning('Please complete the form!');
      return;
    }

    console.log(this.project());

    this.apiService.post(`Project/`, this.project()).subscribe({
      next: (res) => {
        this.snackService.openSuccess(`Project added!`);
        this.onClose();
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
