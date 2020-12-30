import { Component, OnInit } from '@angular/core';
import { LinkModel } from 'src/app/models/LinkModel';
import { AccessService } from 'src/app/services/access.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private links: LinkModel[];

  constructor(private accessService: AccessService) { }

  ngOnInit(): void {

    this.accessService.get<LinkModel[]>(this.accessService.startPoint)
      .subscribe(x=> this.links = x);
  }

  check() {
    this.accessService.get<any>('https://localhost:44370/Home/Index').subscribe(x => alert('Home sweet home!'));
  }

  createNote(titel:string){

    var target = this.links.filter(x=> x.rel == "Note" && x.httpMethod == "PUT")[0]
    this.accessService.put(target.url, titel).subscribe(x=> alert('Ok created.') );
  }

}
