import { Component, OnInit } from '@angular/core';
import { WebAuthService } from 'src/app/services/web-auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  title = 'Sternzeit-App';
  webAuthAvailable = false;
  username = '';

  isLogedIn = false;

  constructor(private webAuth: WebAuthService) {

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

  logout(){
    this.webAuth.logout();
  }


}
