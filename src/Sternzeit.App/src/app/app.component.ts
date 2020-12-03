import { Component, OnInit, Inject } from '@angular/core';
import { access } from 'fs';
import { pipe } from 'rxjs';
import { AccessService } from './access.service';
import { WebAuthService } from './web-auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {

  title = 'Sternzeit-App';
  webAuthAvailable = false;
  username = '';

  isLogedIn = false;

  constructor(private webAuth: WebAuthService, private access: AccessService) {

    webAuth.LogedIn.subscribe(x => this.isLogedIn = true);
  }

  ngOnInit(): void {
    this.webAuthAvailable = this.webAuth.isAvailable();
  }

  register() {
    this.webAuth.loadPreconditions(this.username).subscribe(x => this.webAuth.register(x));
  }

  login() {
    this.webAuth.loadPreconditionLogin(this.username).subscribe(x => this.webAuth.login(x));
  }

  check() {
    this.access.get<any>('https://localhost:44370/Home/Index').subscribe(x => alert('Home sweet home!'));
  }

}
