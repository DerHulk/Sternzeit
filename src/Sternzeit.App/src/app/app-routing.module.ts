import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppGuard } from './app.guard';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { NoteComponent } from './components/note/note.component';

const routes: Routes = [

  { path: 'login', component: LoginComponent,  },
  { path: 'home', component: HomeComponent, canActivate: [AppGuard] },
  { path: 'note', component: NoteComponent, canActivate: [AppGuard] },
  { path: '**', component: LoginComponent },
  { path: '', component: LoginComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
