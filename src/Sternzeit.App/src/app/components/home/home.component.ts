import { Component, OnInit } from '@angular/core';
import { HttpVerbs, RelTypes } from 'src/app/constant';
import { LinkModel } from 'src/app/models/linkModel';
import { AccessService } from 'src/app/services/access.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  private links: LinkModel[];
  public NoteLinks: LinkModel[];

  constructor(private accessService: AccessService) { }

  ngOnInit(): void {

    this.accessService.get<LinkModel[]>(this.accessService.startPoint)
      .subscribe(x=> {
        this.links = x;
        this.NoteLinks = (x.filter(x=> x.rel == RelTypes.Note && x.httpMethod == HttpVerbs.Get));
      });
  }

  createNote(titel:string){

    var target = this.links.filter(x=> x.rel == "Note" && x.httpMethod == "PUT")[0]
    this.accessService.put(target.url, titel).subscribe(x=> alert('Ok created.') );
  }

}
