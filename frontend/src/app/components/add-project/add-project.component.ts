import { Component, inject, signal } from '@angular/core';
import { CdkDrag, CdkDropList, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { MatIcon } from '@angular/material/icon';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ProjectViewDTO, StageViewDTO } from '../../utils/interfaces/ProjectView';

@Component({
  selector: 'app-add-project',
  templateUrl: './add-project.component.html',
  styleUrl: './add-project.component.scss',
  imports: [CdkDrag, CdkDropList, MatAnchor, MatButtonModule, MatIcon, MatFormFieldModule, MatInputModule, MatSelectModule]
})
export class AddProjectComponent {
  project = signal<ProjectViewDTO>({
    name: '',
    description: ''
  })
  dialogRef = inject(MatDialogRef<AddProjectComponent>);

  drop(event: CdkDragDrop<StageViewDTO[] | undefined>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data as any, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data as any,
          event.container.data as any,
          event.previousIndex,
          event.currentIndex);
    }
  }

  onClose(){
    this.dialogRef.close();
  }
}
