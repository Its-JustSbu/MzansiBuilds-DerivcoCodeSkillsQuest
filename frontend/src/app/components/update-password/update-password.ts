import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { updatePasswordView } from '../../utils/interfaces/updatePasswordView';
import { form, FormField, required, validate } from '@angular/forms/signals';
import { Storage } from '../../utils/storage';
import { Api } from '../../utils/services/api';
import { Messagebox } from '../../utils/messagebox';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-password',
  imports: [MatFormFieldModule, MatInputModule, MatIcon, MatButtonModule, FormsModule, FormField],
  templateUrl: './update-password.html',
  styleUrl: './update-password.scss',
})
export class UpdatePassword {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  dialogRef = inject(MatDialogRef);

  password = signal<updatePasswordView>({
    oldPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  passwordForm = form(this.password, (path) => {
    required(path.oldPassword, { message: 'Old Password is required' });
    required(path.newPassword, { message: 'Password is required' });
    required(path.confirmPassword, { message: 'Please confirm password' });
    validate(path.confirmPassword, ({ value, valueOf }) => {
      const confirmPassword = value(); // Current field value
      const password = valueOf(path.newPassword); // Another field's value

      if (confirmPassword !== password) {
        return {
          kind: 'passwordMismatch',
          message: 'Passwords do not match',
        };
      }

      return null;
    });
  });

  onSubmit() {
    if (!this.passwordForm().valid()) {
      this.snackService.openWarning('Please ensure fields are completed!');
      return;
    }

    this.apiService.post('User/Password', this.password()).subscribe({
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
