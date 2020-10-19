import { Injectable } from '@angular/core';

interface WebAuthWindow {
  PublicKeyCredential?: any;

}

declare var window: WebAuthWindow;

@Injectable({
  providedIn: 'root'
})
export class WebAuthService {

  constructor() {

  }

  public isAvailable(): Boolean {
    return window.PublicKeyCredential ? true : false;
  }

  public register(challenge:string): void {

    let keyType: PublicKeyCredentialType;
    navigator.credentials.create({
      publicKey: {
        challenge: null,
        // Request ES256 algorithm ???
        pubKeyCredParams: [{ alg: -7, type: keyType }],
        rp: { id: 'relaying-party', name: '' },
        user: { name: '', displayName: '', id: new TextEncoder().encode("xxxx") }
      }
    }).then((credential) => {

      var publicKeyCredentialToJSON = (pubKeyCred) => {
        if (pubKeyCred instanceof Array) {
          let arr = [];
          for (let i of pubKeyCred)
            arr.push(publicKeyCredentialToJSON(i));

          return arr;
        }

        if (pubKeyCred instanceof ArrayBuffer) {
          return btoa(String.fromCharCode.apply(null, new Uint8Array(pubKeyCred)));
        }

        if (pubKeyCred instanceof Object) {
          let obj = {};

          for (let key in pubKeyCred) {
            obj[key] = publicKeyCredentialToJSON(pubKeyCred[key]);
          }

          return obj;
        }

        return pubKeyCred;
      }

      var credentialsAsJson = publicKeyCredentialToJSON(credential);


    }
}
