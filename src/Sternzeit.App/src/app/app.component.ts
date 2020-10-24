import { Component, OnInit, Inject } from '@angular/core';
import { pipe } from 'rxjs';
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

  constructor(private webAuth: WebAuthService) {

  }

  ngOnInit(): void {
    this.webAuthAvailable = this.webAuth.isAvailable();
  }

  register() {
    this.webAuth.loadPreconditions(this.username).subscribe(x => this.webAuth.register(x));
  }

}
