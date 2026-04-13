import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../utils/api';
import { Messagebox } from '../../utils/messagebox';
import { userView } from '../../utils/interfaces/userView';
import { email, form, required, validate, FormField } from '@angular/forms/signals';
import { Storage } from '../../utils/storage';

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    RouterLink,
    FormField
],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  snackService = inject(Messagebox);
  router = inject(Router);
  storageService = inject(Storage);
  apiService = inject(Api);

  user = signal<userView>({
    name: '',
    surname: '',
    username: '',
    emailAddress: '',
    password: '',
    confirmPassword: '',
  });

  userForm = form(this.user, (path) => {
    required(path.name, { message: 'Name is required' });
    required(path.surname, { message: 'Surname is required' });
    required(path.username, { message: 'Username is required' });
    required(path.emailAddress, { message: 'Email is required' });
    email(path.emailAddress);
    required(path.password, { message: 'Password is required' });
    required(path.confirmPassword, { message: 'Please confirm password' });
    validate(path.confirmPassword, ({ value, valueOf }) => {
      const confirmPassword = value(); // Current field value
      const password = valueOf(path.password); // Another field's value

      if (confirmPassword !== password) {
        return {
          kind: 'passwordMismatch',
          message: 'Passwords do not match',
        };
      }

      return null;
    });
  });

  register() {
    if (!this.userForm().valid()) {
      this.snackService.openWarning('Please ensure fields are completed!');
      return;
    }

    this.apiService.post('user', this.user()).subscribe({
      next: () => {
        this.snackService.openSuccess("User registered Successfully!");
        this.router.navigate(['/login']);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
