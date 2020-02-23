import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppSettings } from '@models/app-settings.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly apiUrl = '/api/v1';
  private _settings;

  constructor(private readonly httpClient: HttpClient) { }

  public get appSettings(): Observable<AppSettings> {
    const httpOptions = {
      headers: new HttpHeaders({
        'X-API-KEY': environment.apiKey
      })
    };

    return !this._settings
      ? this.httpClient.get<AppSettings>(`${this.apiUrl}/settings`, httpOptions)
        .pipe(map((result: AppSettings) => {
          this._settings = result;
          return result;
        }))
      : this._settings;
  }
}
