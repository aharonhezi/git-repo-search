import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GitHubRepo } from '../../shared/models/github-repo.model';

@Injectable({
  providedIn: 'root'
})
export class BookmarksService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/bookmarks`;

  constructor(private http: HttpClient) {}

  getBookmarks(): Observable<GitHubRepo[]> {
    return this.http.get<GitHubRepo[]>(this.apiUrl);
  }

  addBookmark(repo: GitHubRepo): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.apiUrl, repo);
  }

  removeBookmark(repoId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${repoId}`);
  }
}

