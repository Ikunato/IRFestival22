import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PicturesApiService {
  private baseUrl = environment.apiBaseUrl + 'pictures';
  private headers = new HttpHeaders().set('Ocp-Aim-Subscription-Key', '21995f4b9e2944309c26710e615aec93');
  constructor(private httpClient: HttpClient) { }

  getAllUrls(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.baseUrl}`,{'headers' : this.headers});
  }

  upload(file: File): Observable<never> {
    const data = new FormData();
    data.set('file', file);

    return this.httpClient.post<never>(`${this.baseUrl}`, data, {'headers' : this.headers});
  }
}
