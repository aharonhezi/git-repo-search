import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GitHubSearchResponse } from '../../shared/models/github-repo.model';

@Injectable({
  providedIn: 'root'
})
export class GitHubService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/github`;

  constructor(private http: HttpClient) {}

  searchRepositories(query: string, perPage: number = 30, page: number = 1): Observable<GitHubSearchResponse> {
    let params = new HttpParams()
      .set('query', query)
      .set('perPage', perPage.toString())
      .set('page', page.toString());

    return this.http.get<GitHubSearchResponse>(`${this.apiUrl}/search`, { params });
  }
}

