import { Component, inject, OnInit, signal } from '@angular/core';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { UpdatePassword } from '../../components/update-password/update-password';
import { deleteModalData, DeletePopUp } from '../../components/delete-pop-up/delete-pop-up';
import { Api } from '../../utils/api';
import { Messagebox } from '../../utils/messagebox';
import { user } from '../../utils/interfaces/entities';
import { form, FormField, required } from '@angular/forms/signals';
import { MatFormFieldModule } from '@angular/material/form-field';
import { updateUserView } from '../../utils/interfaces/updateUserView';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
  imports: [
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatRadioModule,
    MatCardModule,
    MatFormFieldModule,
    FormField,
  ],
})
export class ProfileComponent implements OnInit {
  ngOnInit(): void {
    this.apiService.get('User/GetCurrentUser').subscribe({
      next: (res: any) => {
        const currentUser = res as user;
        this.user.update(() => {
          const u: updateUserView = {
            name: currentUser.name,
            surname: currentUser.surname,
            username: currentUser.username
          } 

          return u;
        });
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
  snackService = inject(Messagebox);
  apiService = inject(Api);
  dialog = inject(MatDialog);

  user = signal<updateUserView>({
    name: '',
    surname: '',
    username: ''
  });

  profileForm = form(this.user, (path) => {
    required(path.name, { message: 'name is required' });
    required(path.surname, { message: 'surname is required' });
    required(path.username, { message: 'username is required' });
  });

  openUpdatePassword() {
    this.dialog.open(UpdatePassword, {
      maxWidth: '95vw',
      maxHeight: '660px',
      height: '50%',
    });
  }

  openDeactivateAccount() {
    this.dialog.open(DeletePopUp, {
      data: {
        message: 'Are you sure you would like to deactivate your account?',
        subMessage:
          'Doing this will revoke any kind of access to the platform and lose all your saved data.',
        apiUri: 'User',
      } as deleteModalData,
    });
  }

  onSubmit(): void {
    if (!this.profileForm().valid()) {
      this.snackService.openWarning('Please ensure fields are completed!');
      return;
    }

    this.apiService.post('User', this.user).subscribe({
      next: (res: any) => {
        this.snackService.openSuccess(res.message as string);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
