import { AfterViewInit, Component, computed, OnInit, signal, ViewChild } from '@angular/core';
import { MatTableModule, MatTable } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { collaboration } from '../../utils/interfaces/entities';
import { MatIcon } from '@angular/material/icon';
import { MatButtonModule } from "@angular/material/button";

@Component({
  selector: 'app-collaborators-list',
  templateUrl: './collaborators-list.component.html',
  styleUrl: './collaborators-list.component.scss',
  imports: [MatTableModule, MatIcon, MatButtonModule],
})
export class CollaboratorsListComponent implements OnInit {
  ngOnInit(): void {
    this.collaboratorsColumn.update(() => Object.keys(this.collaborators()[0]));
  }

  columns = computed(() => {
    const arr = Object.keys(this.collaborators()[0]);
    arr.push('actions');
    return arr;
  });

  collaborators = signal<collaboration[]>([
    {
      joinedAt: new Date(),
      isOwner: false,
    },
  ]);
  collaboratorsColumn = signal<string[]>([]);
  editAction(element:any){

  }
  deleteAction(element: any){

  }
}
