import { Component, inject } from '@angular/core';

import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { UpdatePassword } from '../../components/update-password/update-password';
import { deleteModalData, DeletePopUp } from '../../components/delete-pop-up/delete-pop-up';

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
    ReactiveFormsModule,
  ],
})
export class ProfileComponent {
  dialog = inject(MatDialog);
  private fb = inject(FormBuilder);
  profileForm = this.fb.group({
    firstName: [null, Validators.required],
    lastName: [null, Validators.required],
    username: [null, Validators.required],
  });

  openUpdatePassword() {
    this.dialog.open(UpdatePassword, {
      maxWidth: "95vw",
      maxHeight: "660px",
      height: "50%"
    });
  }

  openDeactivateAccount() {
    this.dialog.open(DeletePopUp, {
      data: {
        message: "Are you sure you would like to deactivate your account?",
        subMessage: "Doing this will revoke any kind of access to the platform and lose all your saved data.",
        apiUri: "{REMEBER API URI}"
      } as deleteModalData
    })
  }

  onSubmit(): void {
    alert('Thanks!');
  }
}
