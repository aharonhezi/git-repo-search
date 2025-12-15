export interface Owner {
  login: string;
  avatarUrl: string;
}

export interface GitHubRepo {
  id: number;
  name: string;
  fullName: string;
  htmlUrl: string;
  description?: string;
  stargazersCount: number;
  owner: Owner;
}

export interface GitHubSearchResponse {
  totalCount: number;
  items: GitHubRepo[];
}

