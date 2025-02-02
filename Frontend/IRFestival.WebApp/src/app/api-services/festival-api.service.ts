import { Injectable } from '@angular/core';
import { Schedule } from '../api/models/schedule.model';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Artist } from '../api/models/artist.model';

@Injectable({
  providedIn: 'root'
})
export class FestivalApiService {
  private baseUrl = environment.apiBaseUrl + 'festival';
  private headers = new HttpHeaders().set('Ocp-Aim-Subscription-Key', '21995f4b9e2944309c26710e615aec93');
  constructor(private httpClient: HttpClient) { }

  getSchedule(): Observable<Schedule> {
    
    return this.httpClient.get<Schedule>(`${this.baseUrl}/lineup`, {'headers' : this.headers});
  }

  getArtists(): Observable<Artist[]> {
    return this.httpClient.get<Artist[]>(`${this.baseUrl}/artists`, {'headers' : this.headers});
  }
}
