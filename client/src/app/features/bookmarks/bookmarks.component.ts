import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BookmarksService } from '../../core/services/bookmarks.service';
import { GitHubRepo } from '../../shared/models/github-repo.model';

@Component({
  selector: 'app-bookmarks',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './bookmarks.component.html',
  styleUrl: './bookmarks.component.css'
})
export class BookmarksComponent implements OnInit {
  bookmarks: GitHubRepo[] = [];
  loading = false;

  constructor(
    private bookmarksService: BookmarksService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadBookmarks();
  }

  loadBookmarks(): void {
    this.loading = true;
    this.bookmarksService.getBookmarks().subscribe({
      next: (bookmarks) => {
        this.bookmarks = bookmarks;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        this.snackBar.open(
          error.error?.message || 'Failed to load bookmarks. Please try again.',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          }
        );
      }
    });
  }

  removeBookmark(repo: GitHubRepo): void {
    this.bookmarksService.removeBookmark(repo.id).subscribe({
      next: () => {
        this.bookmarks = this.bookmarks.filter(b => b.id !== repo.id);
        this.snackBar.open('Bookmark removed', 'Close', {
          duration: 2000
        });
      },
      error: () => {
        this.snackBar.open('Failed to remove bookmark. Please try again.', 'Close', {
          duration: 3000
        });
      }
    });
  }

  openRepository(repo: GitHubRepo): void {
    window.open(repo.htmlUrl, '_blank');
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img) {
      img.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiNjY2MiLz48dGV4dCB4PSI1MCUiIHk9IjUwJSIgZm9udC1zaXplPSIxNCIgZmlsbD0iIzk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPj88L3RleHQ+PC9zdmc+';
    }
  }
}

