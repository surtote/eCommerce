import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent }  from './app.component';
import { CadenasService } from './Cadenas/CadenaService';
import { CadenaListaComponent } from './Cadenas/CadenaLista.component';
import { HotelService } from './Hoteles/HotelService';
import { HotelComponent } from './Hoteles/Hotel.component';
import { HotelListaComponent } from './Hoteles/HotelLista.component';
import { CadenaComponent } from './Cadenas/Cadena.component';
const appRutas: Routes = [
    { path: 'Cadena', component: CadenaListaComponent },
    { path: 'Hotel/:Nombre', component: HotelComponent },
    { path: 'Cadena/:nombre', component: CadenaListaComponent },
    { path: 'Hotel', component: HotelListaComponent },


]
@NgModule({
    imports: [BrowserModule, HttpModule, RouterModule.forRoot(appRutas)],
  declarations: [ AppComponent, CadenaListaComponent, HotelComponent, HotelListaComponent, CadenaComponent, ],
    bootstrap: [AppComponent],
    providers:[CadenasService, HotelService]
})
export class AppModule { }
