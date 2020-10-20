import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { stringify } from 'querystring';
import { ThrowStmt } from '@angular/compiler';
import { environment } from 'src/environments/environment';
import { promise } from 'protractor';
import { Observable, pipe } from 'rxjs';
import { map } from 'rxjs/operators';

interface WebAuthWindow {
  PublicKeyCredential?: any;
}

class RegisterPreconditions {
  Challenge: string;
  RelayingPartyId: string;
  RelayingPartyName: string;

  UserName: string;
  UserDisplayName: string;
  UserId: string;

  RegisterUrl: string;
}

declare var window: WebAuthWindow;

@Injectable({
  providedIn: 'root'
})
export class WebAuthService {

  constructor(private http: HttpClient) {
  }

  public isAvailable(): Boolean {
    return window.PublicKeyCredential ? true : false;
  }

  public loadPreconditions(userName: string): Observable<RegisterPreconditions> {

    return this.http.get<RegisterPreconditions>(environment.registrationEndPoint)
      .pipe(map(x => { x.UserName = userName; return x; }));
  }

  public register(preconditions: RegisterPreconditions): void {

    let keyType: PublicKeyCredentialType;
    const challengeBuffer = this.ConvertToBuffer(preconditions.Challenge);

    navigator.credentials.create({
      publicKey: {
        // Request ES256 algorithm
        //https://developer.mozilla.org/en-US/docs/Web/API/PublicKeyCredentialCreationOptions/pubKeyCredParams
        pubKeyCredParams: [{ alg: -7, type: keyType }],
        challenge: challengeBuffer,
        rp: {
          id: preconditions.RelayingPartyId,
          name: preconditions.RelayingPartyName
        },
        user: {
          id: new TextEncoder().encode(preconditions.UserId),
          name: preconditions.UserName,
          displayName: preconditions.UserName,
        }
      }
    }).then((credential) => {
      const credentialsAsJson = this.ConvertToJson(credential);

      this.http.post<any>(preconditions.RegisterUrl, credentialsAsJson, { observe: 'response' })
        .subscribe(x => {

          if (x.ok) {
            //we expected a back url in response.
          }
          else {
           throw new Error('That have not worked.');
          }

        });

    });
  }

  private ConvertToBuffer(value: string) {
    const byteCharacters = atob(value);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    return new Uint8Array(byteNumbers);
  }

  private ConvertToJson(credentials: Credential) {
    if (credentials instanceof Array) {
      const arr = [];
      for (let i of credentials) {
        arr.push(this.ConvertToJson(i));
      }

      return arr;
    }

    if (credentials instanceof ArrayBuffer) {
      return btoa(String.fromCharCode.apply(null, new Uint8Array(credentials)));
    }

    if (credentials instanceof Object) {
      const obj = {};

      for (let key in credentials) {
        obj[key] = this.ConvertToJson(credentials[key]);
      }

      return obj;
    }

    return credentials;
  }

}
