import { Component, OnInit, Inject } from '@angular/core';
import { pipe } from 'rxjs';
import { AccessService } from './services/access.service';
import { WebAuthService, } from './services/web-auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent implements OnInit {

  title = 'Sternzeit-App';

  constructor() {
  }

  ngOnInit(): void {
  }
}
