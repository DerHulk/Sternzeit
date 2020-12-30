import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WebAuthService } from './web-auth.service';



@Injectable({
  providedIn: 'root'
})
export class AccessService {

  private authToken: string;
  public startPoint: string;

  constructor(private http: HttpClient,
    private webAuthService: WebAuthService) {


    this.webAuthService.LogedIn.subscribe(x => {
      if (!x) {
        return;
      }
      this.authToken = x.token;
      this.startPoint = x.startPoint;

    });

    this.webAuthService.LogedOut.subscribe(x=> {

      if(x == null)
        return;

      this.authToken = null;
      this.startPoint = null;
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

    return this.http.post<T>(url, value, options);
  }

  public put<T>(url: string, value: any): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.put<T>(url, value, options);
  }

  public delte<T>(url: string): Observable<T> {
    const options = {};
    this.setAuthHeader(options);

    return this.http.delete<T>(url, options);
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
