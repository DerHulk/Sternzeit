import { Component, OnInit } from '@angular/core';
import { AccessService } from 'src/app/services/access.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private accessService: AccessService) { }

  ngOnInit(): void {
  }

  check() {
    this.accessService.get<any>('https://localhost:44370/Home/Index').subscribe(x => alert('Home sweet home!'));
  }

}
