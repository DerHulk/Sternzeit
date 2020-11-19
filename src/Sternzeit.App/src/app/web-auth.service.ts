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
  challenge: string;
  relayingPartyId: string;
  relayingPartyName: string;

  userName: string;
  userDisplayName: string;
  userId: string;

  registerUrl: string;
}

class LoginPreconditions {
  keyId: string;
  challenge: string;
  relyingPartyId: string;
  loginCallbackUrl: string;
}

declare var window: WebAuthWindow;

@Injectable({
  providedIn: 'root'
})
export class WebAuthService {

  constructor(private http: HttpClient) {
  }

  public isAvailable(): boolean {
    return window.PublicKeyCredential ? true : false;
  }

  public loadPreconditions(userName: string): Observable<RegisterPreconditions> {

    return this.http.get<RegisterPreconditions>(environment.registrationEndPoint + '?username=' + userName)
      .pipe(map(x => { x.userName = userName; return x; }));
  }

  public register(preconditions: RegisterPreconditions): void {

    const challengeBuffer = this.convertToBuffer(preconditions.challenge);

    navigator.credentials.create({
      publicKey: {
        // Request ES256 algorithm
        //https://developer.mozilla.org/en-US/docs/Web/API/PublicKeyCredentialCreationOptions/pubKeyCredParams
        pubKeyCredParams: [{ alg: -7, type: 'public-key' }],
        challenge: challengeBuffer,
        rp: {
          id: preconditions.relayingPartyId,
          name: preconditions.relayingPartyName
        },
        user: {
          id: new TextEncoder().encode(preconditions.userId),
          name: preconditions.userName,
          displayName: preconditions.userName,
        }
      }
    }).then((credential) => {
      const credentialsAsJson = this.convertToJson(credential);

      this.http.post<any>(preconditions.registerUrl, credentialsAsJson, { observe: 'response' })
        .subscribe(x => {

          if (x.ok) {
            //we expected a back url in response.
            alert('Great. You are registered. :-)');
          }
          else {
            throw new Error('That have not worked.');
          }

        });

    });
  }

  private convertToBuffer(value: string) {
    const byteCharacters = atob(value);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const result = new Uint8Array(byteNumbers);

    return result;
  }

  private bin2String(array): string {
    return String.fromCharCode.apply(String, array);
  }

  private convertToJson(credentials: Credential) {
    if (credentials instanceof Array) {
      const arr = [];
      for (let i of credentials) {
        arr.push(this.convertToJson(i));
      }

      return arr;
    }

    if (credentials instanceof ArrayBuffer) {
      return btoa(String.fromCharCode.apply(String, new Uint8Array(credentials)));
    }

    if (credentials instanceof Object) {
      const obj = {};

      for (let key in credentials) {
        obj[key] = this.convertToJson(credentials[key]);
      }

      return obj;
    }

    return credentials;
  }

  public loadPreconditionLogin(userName): Observable<LoginPreconditions> {
    return this.http.get<LoginPreconditions>(environment.loginEndPoint + '?username=' + userName)
      .pipe(map(x => x));
  }

  public login(preconditions: LoginPreconditions) {

    const challenge = this.convertToBuffer(preconditions.challenge);
    const key = this.convertToBuffer(preconditions.keyId);

    const allowCredentials = [
      {
        type: <PublicKeyCredentialType>'public-key',
        id: key,
        transports: <AuthenticatorTransport[]>['usb']
      }
    ];

    const publicKey = { challenge, allowCredentials, rpId: preconditions.relyingPartyId };

    navigator.credentials.get({ publicKey }).then((credential) => {

      const credentialsAsJson = this.convertToJson(credential);

      this.http.post<any>(preconditions.loginCallbackUrl, credentialsAsJson, { observe: 'response' })
        .subscribe(x => {

          if (x.ok) {
            //we expected a back url in response.
            alert('Great. You are loged in. :-)');
          }
          else {
            throw new Error('That have not worked.');
          }

        });

    });

  }

}
