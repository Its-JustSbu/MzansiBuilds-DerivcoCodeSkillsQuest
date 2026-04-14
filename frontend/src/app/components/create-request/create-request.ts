import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { collaboration, project } from '../../utils/interfaces/entities';
import { Messagebox } from '../../utils/messagebox';
import { Api } from '../../utils/services/api';

@Component({
  selector: 'app-create-request',
  imports: [MatDialogModule, MatFormFieldModule, FormsModule, MatAnchor, MatButtonModule],
  templateUrl: './create-request.html',
  styleUrl: './create-request.scss',
})
export class CreateRequest {
  project: project = inject(MAT_DIALOG_DATA);
  snackService = inject(Messagebox);
  apiService = inject(Api);
  dialogRef = inject(MatDialogRef<CreateRequest>);
  onClose() {
    this.dialogRef.close();
  }
  request = signal<collaboration>({
    requestStatus: {
      id: 1,
      name: "Pending"
    },
    collaboratorType: {
      id: 2,
      name: "Contributor"
    },
    isOwner: false
  })
  sendRequest() {
    this.apiService.post(`Collaborator/${this.project.id}`, this.request()).subscribe({
      next: (res: any) => {
        this.snackService.openSuccess("Request Successfully Sent!");
        this.onClose();
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
