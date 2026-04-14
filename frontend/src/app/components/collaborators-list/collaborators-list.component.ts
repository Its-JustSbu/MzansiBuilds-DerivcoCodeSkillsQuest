import {
  AfterViewInit,
  Component,
  computed,
  inject,
  Input,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MatTableModule, MatTable } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { collaboration } from '../../utils/interfaces/entities';
import { MatIcon } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';

interface collaboratorsView {
  no: number;
  username: string;
  email: string;
  joinedAt: Date;
  type: number;
}
@Component({
  selector: 'app-collaborators-list',
  templateUrl: './collaborators-list.component.html',
  styleUrl: './collaborators-list.component.scss',
  imports: [MatTableModule, MatIcon, MatButtonModule, DatePipe],
})
export class CollaboratorsListComponent implements OnInit {
  snackService = inject(Messagebox);
  apiService = inject(Api);
  @Input({ required: true }) data!: collaboration[];

  ngOnInit(): void {
    this.collaborators.update(() => {
      const temp: collaboratorsView[] = [];
      this.data.forEach((d) =>
        temp.push({
          no: d.id as number,
          username: d.user?.username as string,
          email: d.user?.emailAddress as string,
          joinedAt: d.joinedAt as Date,
          type: d.requestStatus?.id as number,
        }),
      );
      return temp;
    });

    this.collaboratorsColumn.update(() =>
      Object.keys({
        username: '',
        email: '',
        joinedAt: new Date(),
      } as collaboratorsView),
    );
  }

  columns = computed(() => {
    const arr = Object.keys({
      username: '',
      email: '',
      joinedAt: new Date(),
    } as collaboratorsView);
    arr.push('actions');
    return arr;
  });

  collaborators = signal<collaboratorsView[]>([]);
  collaboratorsColumn = signal<string[]>([]);

  acceptAction(element: collaboratorsView) {
    const request = signal<collaboration>({
      requestStatus: {
        id: 2,
        name: 'Approved',
      },
      collaboratorType: {
        id: 2,
        name: 'Contributor',
      },
      isOwner: false,
    });
    this.apiService.put(`Collaborator/${element.no}`, request()).subscribe({
      next: () => {
        this.snackService.openSuccess('Request Successfully Accepted!');
        this.collaborators.update((arr) =>
          arr.map((e) =>
            e.no === element.no
              ? {
                  ...e,
                  type: 2,
                }
              : e,
          ),
        );
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
  rejectAction(element: collaboratorsView) {
    const request = signal<collaboration>({
      requestStatus: {
        id: 3,
        name: 'Rejected',
      },
      collaboratorType: {
        id: 2,
        name: 'Contributor',
      },
      isOwner: false,
    });
    this.apiService.put(`Collaborator/${element.no}`, request()).subscribe({
      next: () => {
        this.snackService.openSuccess('Request Successfully Rejected!');
        this.collaborators.update((arr) =>
          arr.map((e) =>
            e.no === element.no
              ? {
                  ...e,
                  type: 2,
                }
              : e,
          ),
        );
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
  normalColumn(str: string) {
    return str
      .replace(/([A-Z])/g, ' $1') // Adds a space before capital letters [1, 2]
      .replace(/^./, (str: string) => str.toUpperCase()); // Optionally capitalizes the first letter
  }
}
