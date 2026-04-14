import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../utils/services/api';
import { loginView } from '../../utils/interfaces/loginView';
import { Storage } from '../../utils/storage';
import { Messagebox } from '../../utils/messagebox';
import { form, FormField, required } from '@angular/forms/signals';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    RouterLink,
    FormField,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  snackService = inject(Messagebox);
  router = inject(Router);
  storageService = inject(Storage);
  apiService = inject(Api);

  user = signal<loginView>({
    username: '',
    password: '',
  });

  userForm = form(this.user, (path) => {
    required(path.username, { message: 'Email is required' });
    required(path.password, { message: 'Password is required' });
  });

  login() {
    if (!this.userForm().valid()) {
      this.snackService.openWarning('Please ensure fields are completed!');
      return;
    }

    this.apiService.post('User/Login', this.user()).subscribe({
      next: (res: any) => {
        this.storageService.setItem('token', res.token);
        this.storageService.setItem('refreshToken', res.refreshToken);
        this.snackService.openSuccess(res.message as string);
        this.router.navigate(['/portal/home']);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
