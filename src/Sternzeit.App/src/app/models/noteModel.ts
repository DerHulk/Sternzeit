import { LinkModel } from "./linkModel";
import { NoteIdModel } from "./noteIdModel";

export class NoteModel {
  Id: NoteIdModel;
  titel:string;
  text: string;
  tags: string[];
  links: LinkModel[];
}
