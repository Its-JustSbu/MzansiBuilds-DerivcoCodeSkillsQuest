import { Component, inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import {
  MilestoneViewDTO,
  ProjectViewDTO,
  StageViewDTO,
  SupportViewDTO,
} from '../../utils/interfaces/ProjectView';
import { MatTableModule } from '@angular/material/table';
import { MatIcon } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CreateRequest } from '../create-request/create-request';
import { CollaboratorsListComponent } from '../collaborators-list/collaborators-list.component';
import { MatDivider } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MilestoneList } from '../milestone-list/milestone-list';
import { MatInputModule } from '@angular/material/input';
import {
  collaboration,
  comment,
  lookup,
  project,
  projectStage,
} from '../../utils/interfaces/entities';
import { DatePipe } from '@angular/common';
import { createCommentView } from '../../utils/interfaces/createCommentView';
import { Messagebox } from '../../utils/services/messagebox';
import { Api } from '../../utils/services/api';
import { form, required, FormField } from '@angular/forms/signals';
import { FormsModule } from '@angular/forms';

export interface projectDialogData {
  isMine: boolean;
  project: project;
  collabs: collaboration[];
}

@Component({
  selector: 'app-project-view',
  imports: [
    MatTableModule,
    MatIcon,
    MatButtonModule,
    MatFormFieldModule,
    CollaboratorsListComponent,
    MatDivider,
    MatListModule,
    MilestoneList,
    MatInputModule,
    DatePipe,
    FormField,
    FormsModule,
  ],
  templateUrl: './project-view.html',
  styleUrl: './project-view.scss',
})
export class ProjectView implements OnInit {
  data: projectDialogData = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<ProjectView>);
  dialog = inject(MatDialog);
  stages = signal<StageViewDTO[]>([]);
  support = signal<SupportViewDTO[]>([]);
  stagesColumns = signal<string[]>([]);
  supportColumns = signal<string[]>([]);
  stagesDataSource = signal<any[]>([]);
  supportDataSource = signal<any[]>([]);
  snackService = inject(Messagebox);
  apiService = inject(Api);
  comments = signal<comment[]>(this.data.project.comments as comment[]);

  constructor() {
    this.stages.update(() => {
      const temp: StageViewDTO[] = [];
      this.data.project.stages?.forEach((stage) => {
        temp.push({
          stageTitle: stage.stageTitle,
          stageNumber: stage.stageNumber,
          stageStatus: stage.stageStatus as lookup,
          milestones: stage.milestones as MilestoneViewDTO[],
        });
      });

      return temp;
    });
    this.support.update(() => {
      const temp: SupportViewDTO[] = [];
      this.data.project.support?.forEach((s) => {
        temp.push({
          description: s.description,
          supportType: s.supportType as lookup,
        });
      });

      return temp;
    });
  }

  ngOnInit(): void {
    if (this.stages().length > 0) {
      this.stagesColumns.update(() => Object.keys(this.stages()[0]));
      this.stagesDataSource.update(() => this.setUpTable(this.stages(), this.stagesColumns()));
    }
    if (this.support().length > 0) {
      this.supportColumns.update(() => Object.keys(this.support()[0]));
      this.supportDataSource.update(() => this.setUpTable(this.support(), this.supportColumns()));
    }
    this.getComments();
  }

  setUpTable(data: any[], columns: string[]): any {
    let newData: any = {};

    columns.forEach((column) => {
      let temp = data.map((c) => c[column]);
      if (!this.IsArray(temp[0])) {
        if (this.isPlainObject(temp[0])) {
          newData = { ...newData, [column]: temp.map((c) => c.name) };
        } else {
          newData = { ...newData, [column]: temp.map((c) => c) };
        }
      } else {
        newData = { ...newData, [column]: temp.map((c) => c.length) };
      }
    });

    const keys = Object.keys(newData);
    const length = newData[keys[0]].length; // Assumes all arrays are same length

    const newTable = Array.from({ length }).map((_, i) => {
      return keys.reduce((acc, key) => {
        acc[key] = newData[key][i];
        return acc;
      }, {} as any);
    });

    return newTable;
  }

  IsArray(arr: any): boolean {
    return Array.isArray(arr);
  }

  isPlainObject(arr: any): boolean {
    return arr !== null && typeof arr === 'object' && !Array.isArray(arr);
  }

  normalColumn(str: string) {
    return str
      .replace(/([A-Z])/g, ' $1') // Adds a space before capital letters [1, 2]
      .replace(/^./, (str: string) => str.toUpperCase()); // Optionally capitalizes the first letter
  }

  requestAccess() {
    this.dialog.open(CreateRequest, {
      data: this.data.project,
    });
  }

  newComment = signal<createCommentView>({
    title: '',
    description: '',
  });

  commentForm = form(this.newComment, (path) => {
    required(path.title, { message: 'title is required.' });
    required(path.description, { message: 'description is required.' });
  });

  addComment() {
    this.apiService
      .post(`Collaborator/${this.data.project.id}/comment`, this.newComment())
      .subscribe({
        next: () => {
          this.snackService.openSuccess('Comment Posted!');
          this.comments.update((arr) => [
            ...arr,
            {
              title: this.newComment().title,
              description: this.newComment().description,
              createdAt: new Date(),
            },
          ]);
          this.commentForm().reset();
        },
        error: (error: any) => {
          this.snackService.openError(error.error.message);
        },
      });
  }

  getComments() {
    this.apiService.get(`Collaborator/${this.data.project.id}/comments`).subscribe({
      next: (res: any) => {
        this.comments.update(() => res as comment[]);
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
