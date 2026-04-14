import { Component, inject, Input, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { MatDialog } from '@angular/material/dialog';
import { projectDialogData, ProjectView } from '../project-view/project-view';
import { CreateRequest } from '../create-request/create-request';
import { UpdateProjectComponent } from '../update-project/update-project.component';
import { deleteModalData, DeletePopUp } from '../delete-pop-up/delete-pop-up';
import { ManageMilestones } from '../manage-milestones/manage-milestones';
import { collaboration, project, projectStage } from '../../utils/interfaces/entities';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-project-card',
  imports: [MatIconModule, MatButtonModule, MatCardModule, MatMenuModule, DatePipe],
  templateUrl: './project-card.html',
  styleUrl: './project-card.scss',
})
export class ProjectCard {
  @Input({ required: true }) isGeneralCard!: boolean;
  @Input({ required: true }) card!: project;
  @Input() collaborations!: collaboration[];
  dialog = inject(MatDialog);

  openDetails() {
    this.dialog.open(ProjectView, {
      data: { project: this.card, isMine: !this.isGeneralCard, collabs: this.collaborations } as projectDialogData,
      maxWidth: '95vw',
      width: '100%',
      maxHeight: '660px',
      height: '100%',
    });
  }

  openEdit() {
    this.dialog.open(UpdateProjectComponent, {
      data: this.card as ProjectViewDTO,
      maxWidth: '95vw',
      width: '100%',
      maxHeight: '660px',
      height: '100%',
    });
  }

  openDelete() {
    this.dialog.open(DeletePopUp, {
      data: {
        message: `Are you sure you would like to delete ${this.card.name}?`,
        subMessage:
          'Once this is done, it can not be undone, all collabs and comments will be removed.',
        apiUri: '{REMEMBER API URI}',
      } as deleteModalData,
      maxWidth: '50vw',
      width: '100%',
      maxHeight: '560px',
    });
  }

  openRequestModal() {
    this.dialog.open(CreateRequest, {
      data: this.card as ProjectViewDTO,
    });
  }

  markAsComplete() {}

  openMilestones() {
    this.dialog.open(ManageMilestones, {
      data: this.card as project,
      maxWidth: '95vw',
      width: '45%',
      maxHeight: '660px',
      height: '100%',
    });
  }
}
