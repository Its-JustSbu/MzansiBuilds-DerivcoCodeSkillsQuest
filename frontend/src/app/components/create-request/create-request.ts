import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ProjectViewDTO } from '../../utils/interfaces/ProjectView';
import { MatAnchor, MatButtonModule } from "@angular/material/button";

@Component({
  selector: 'app-create-request',
  imports: [MatDialogModule, MatFormFieldModule, FormsModule, MatAnchor, MatButtonModule],
  templateUrl: './create-request.html',
  styleUrl: './create-request.scss',
})
export class CreateRequest {
  project: ProjectViewDTO = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<CreateRequest>);
  onClose() {
    this.dialogRef.close();
  }
  sendRequest() {}
}
