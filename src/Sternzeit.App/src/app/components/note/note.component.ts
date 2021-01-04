import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoteModel } from 'src/app/models/noteModel';
import { AccessService } from 'src/app/services/access.service';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.css']
})
export class NoteComponent implements OnInit {

  note: NoteModel;

  constructor(private route: ActivatedRoute, private accessService: AccessService) { }

  ngOnInit(): void {

    const url = this.route.snapshot.paramMap.get('sourceUrl');
    this.accessService.get<NoteModel>(url).subscribe(x=> this.note = x);
  }

}
