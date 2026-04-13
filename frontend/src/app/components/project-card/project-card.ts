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
import { DeletePopUp } from '../delete-pop-up/delete-pop-up';
import { ManageMilestones } from '../manage-milestones/manage-milestones';

@Component({
  selector: 'app-project-card',
  imports: [MatIconModule, MatButtonModule, MatCardModule, MatMenuModule],
  templateUrl: './project-card.html',
  styleUrl: './project-card.scss',
})
export class ProjectCard {
  @Input({ required: true }) isGeneralCard!: boolean;
  @Input({ required: true }) card!: ProjectViewDTO;
  dialog = inject(MatDialog);

  openDetails() {
    this.dialog.open(ProjectView, {
      data: { project: this.card, isMine: !this.isGeneralCard } as projectDialogData,
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
      data: this.card as ProjectViewDTO,
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
      maxWidth: '95vw',
      width: '45%',
      maxHeight: '660px',
      height: '100%',
    });
  }
}
