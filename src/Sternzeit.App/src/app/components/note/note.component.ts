import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NoteModel } from 'src/app/models/noteModel';
import { AccessService } from 'src/app/services/access.service';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss']
})
export class NoteComponent implements OnInit {

  note: NoteModel;
  sourceUrl: string;
  editText: boolean;

  constructor(private route: ActivatedRoute, private accessService: AccessService) { }

  ngOnInit(): void {

    this.editText = false;
    this.sourceUrl = this.route.snapshot.paramMap.get('sourceUrl');
    this.accessService.get<NoteModel>(this.sourceUrl).subscribe(x=> this.note = x);
  }

  save(): void {
    this.accessService.patch(this.sourceUrl, this.note).subscribe(x=> x);
  }

}
