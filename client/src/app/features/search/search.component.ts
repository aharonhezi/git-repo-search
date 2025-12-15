import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { GitHubService } from '../../core/services/github.service';
import { BookmarksService } from '../../core/services/bookmarks.service';
import { GitHubRepo } from '../../shared/models/github-repo.model';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent implements OnInit {
  searchForm!: FormGroup;
  repositories: GitHubRepo[] = [];
  loading = false;
  totalCount = 0;
  bookmarkedIds = new Set<number>();

  constructor(
    private fb: FormBuilder,
    private githubService: GitHubService,
    private bookmarksService: BookmarksService,
    private snackBar: MatSnackBar
  ) {
    this.searchForm = this.fb.group({
      query: ['']
    });
  }

  ngOnInit(): void {
    this.loadBookmarks();
  }

  private loadBookmarks(): void {
    this.bookmarksService.getBookmarks().subscribe({
      next: (bookmarks) => {
        this.bookmarkedIds = new Set(bookmarks.map(b => b.id));
      },
      error: () => {
        // Silently fail - bookmarks will be loaded when needed
      }
    });
  }

  onSearch(): void {
    const query = this.searchForm.value.query?.trim();
    if (!query) {
      this.snackBar.open('Please enter a search query', 'Close', {
        duration: 3000
      });
      return;
    }

    this.loading = true;
    this.repositories = [];

    this.githubService.searchRepositories(query).subscribe({
      next: (response) => {
        this.repositories = response.items;
        this.totalCount = response.totalCount;
        this.loading = false;
        this.loadBookmarks();

        if (response.items.length === 0) {
          this.snackBar.open('No repositories found', 'Close', {
            duration: 3000
          });
        }
      },
      error: (error) => {
        this.loading = false;
        const message = error.status === 429
          ? 'GitHub API rate limit exceeded. Please try again later.'
          : error.error?.message || 'Failed to search repositories. Please try again.';
        
        this.snackBar.open(message, 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top'
        });
      }
    });
  }

  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.onSearch();
    }
  }

  toggleBookmark(repo: GitHubRepo): void {
    if (this.isBookmarked(repo.id)) {
      this.removeBookmark(repo.id);
    } else {
      this.addBookmark(repo);
    }
  }

  private addBookmark(repo: GitHubRepo): void {
    this.bookmarksService.addBookmark(repo).subscribe({
      next: () => {
        this.bookmarkedIds.add(repo.id);
        this.snackBar.open('Repository bookmarked!', 'Close', {
          duration: 2000
        });
      },
      error: (error) => {
        const message = error.status === 409
          ? 'Repository is already bookmarked'
          : 'Failed to bookmark repository. Please try again.';
        
        this.snackBar.open(message, 'Close', {
          duration: 3000
        });
      }
    });
  }

  private removeBookmark(repoId: number): void {
    this.bookmarksService.removeBookmark(repoId).subscribe({
      next: () => {
        this.bookmarkedIds.delete(repoId);
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

  isBookmarked(repoId: number): boolean {
    return this.bookmarkedIds.has(repoId);
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

