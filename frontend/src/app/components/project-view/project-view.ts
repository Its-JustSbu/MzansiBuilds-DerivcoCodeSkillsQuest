import { Component, inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ProjectViewDTO, StageViewDTO, SupportViewDTO } from '../../utils/interfaces/ProjectView';
import { MatTableModule } from '@angular/material/table';
import { MatIcon } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CreateRequest } from '../create-request/create-request';
import { CollaboratorsListComponent } from '../collaborators-list/collaborators-list.component';

export interface projectDialogData {
  isMine: boolean;
  project: ProjectViewDTO;
}

@Component({
  selector: 'app-project-view',
  imports: [
    MatTableModule,
    MatIcon,
    MatButtonModule,
    MatFormFieldModule,
    CollaboratorsListComponent,
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

  constructor() {
    this.stages.update(() => this.data.project.stages as StageViewDTO[]);
    this.support.update(() => this.data.project.support as SupportViewDTO[]);
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

  onClose() {
    this.dialogRef.close();
  }
  openRequestModal() {}
}
