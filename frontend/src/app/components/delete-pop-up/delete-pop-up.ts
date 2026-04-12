import { Component, inject } from '@angular/core';
import { MatAnchor } from "@angular/material/button";
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface deleteModalData {
  message: string;
  subMessage: string;
  apiUri: string;
}

@Component({
  selector: 'app-delete-pop-up',
  imports: [MatAnchor],
  templateUrl: './delete-pop-up.html',
  styleUrl: './delete-pop-up.scss',
})
export class DeletePopUp {
  data: deleteModalData = inject(MAT_DIALOG_DATA);
}
