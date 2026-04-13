import { Component, inject } from '@angular/core';
import { MatAnchor } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';

export interface deleteModalData {
  message: string;
  subMessage: string;
  apiUri: string;
  data?: any;
}

@Component({
  selector: 'app-delete-pop-up',
  imports: [MatAnchor],
  templateUrl: './delete-pop-up.html',
  styleUrl: './delete-pop-up.scss',
})
export class DeletePopUp {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  data: deleteModalData = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<DeletePopUp>);

  onSubmit() {
    this.apiService.delete(this.data.apiUri, this.data.data).subscribe({
      next: (res: any) => {
        this.snackService.openSuccess(res.message as string);
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
