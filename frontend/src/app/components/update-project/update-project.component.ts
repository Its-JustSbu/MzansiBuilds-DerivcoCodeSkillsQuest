import { Component, inject } from '@angular/core';
import { CdkDrag, CdkDropList, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ProjectViewDTO, StageViewDTO, SupportViewDTO } from '../../utils/interfaces/ProjectView';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-update-project',
  templateUrl: './update-project.component.html',
  styleUrl: './update-project.component.scss',
  imports: [CdkDrag, CdkDropList, MatButtonModule, MatIcon, MatFormFieldModule, MatInputModule, MatSelectModule]
})
export class UpdateProjectComponent {
  project: ProjectViewDTO = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<UpdateProjectComponent>);

  drop(event: CdkDragDrop<StageViewDTO[]>): void {
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
