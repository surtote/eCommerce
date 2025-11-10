import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { AppComponent }  from './app.component';
import { EquipoComponent } from './Equipo/Equipo.component';
import { EquipoListaComponent } from './Equipo/EquipoLista.component';

@NgModule({
  imports:      [ BrowserModule, HttpModule ],
  declarations: [ AppComponent, EquipoComponent, EquipoListaComponent ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
