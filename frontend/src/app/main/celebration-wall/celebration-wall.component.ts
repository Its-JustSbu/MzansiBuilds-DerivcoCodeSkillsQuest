import { Component, inject, OnInit, signal } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { MatGridListModule } from '@angular/material/grid-list';
import { CelebrationCard } from '../../components/celebration-card/celebration-card';
import { project } from '../../utils/interfaces/entities';
import { Api } from '../../utils/services/api';
import { Messagebox } from '../../utils/services/messagebox';
import { Storage } from '../../utils/services/storage';

@Component({
  selector: 'app-celebration-wall',
  templateUrl: './celebration-wall.component.html',
  styleUrl: './celebration-wall.component.scss',
  imports: [MatGridListModule, CelebrationCard],
})
export class CelebrationWallComponent implements OnInit {
  ngOnInit(): void {
    this.getCelebrations(this.pageNumber());
  }
  snackService = inject(Messagebox);
  apiService = inject(Api);
  pageNumber = signal<number>(1);
  private breakpointObserver = inject(BreakpointObserver);
  cols = 2;

  getCelebrations(page: number) {
    this.apiService.get(`Project/Celebrations/${page}`).subscribe({
      next: (res: any) => {
        this.cards.update(() => res as project[]);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }

  constructor() {
    /** Based on the screen size, switch from standard to one column per row */
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.cols = result.matches ? 1 : 2; // 1 column for mobile, 2 for desktop
    });
  }
  cards = signal<project[]>([]);
}
