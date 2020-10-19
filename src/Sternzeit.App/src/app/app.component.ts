import { Component, OnInit, Inject } from '@angular/core';
import { WebAuthService } from './web-auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {

  title = 'Sternzeit-App';

  constructor(private webAuth: WebAuthService) {

  }

  ngOnInit(): void {

    if (!this.webAuth.isAvailable()) {
      alert('Browser does not support WebAuthn.');
    }
    else {
      alert('Browser support WebAuthn.');
    }
    // if (!window.PublicKeyCredential) {
    //   alert('Browser does not support WebAuthn.');
    //   return;
    // }
  }

}
