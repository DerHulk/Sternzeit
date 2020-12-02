import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { WebAuthService } from './web-auth.service';


@Injectable({
  providedIn: 'root'
})
export class AccessService {

  private authToken: string;
  private expiresAt: Date;

  constructor(private http: HttpClient,
              webAuthService: WebAuthService) {

    webAuthService.LogedIn.subscribe(x => {
      this.authToken = x.token,
        this.expiresAt = new Date(x.expiresAt);
    });
  }

  public get<T>(url: string): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.get<T>(url, options);
  }

  public post<T>(url: string, value: T): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.post<T>(url, value);
  }

  public put<T>(url: string, value: T): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.put<T>(url, value);
  }

  public delte<T>(url: string): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.delete<T>(url);
  }

  private setAuthHeader(options: any) {
    const httpOptions = {
      headers: new HttpHeaders()
    };

    if (this.authToken) {
      options['headers'] = new HttpHeaders()
        .append('authorization', 'Bearer ' + this.authToken);
    }

    return httpOptions;
  }
}
