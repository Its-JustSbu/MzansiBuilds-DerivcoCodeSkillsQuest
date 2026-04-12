import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-update-password',
  imports: [MatFormFieldModule, MatInputModule, MatIcon, MatButtonModule],
  templateUrl: './update-password.html',
  styleUrl: './update-password.scss',
})
export class UpdatePassword {
  dialogRef = inject(MatDialogRef);

  onClose(){
    this.dialogRef.close();
  }
}
