import { Component, OnInit } from '@angular/core';
import { Cadena } from './Cadena';
import { CadenasService } from './CadenaService';
@Component({
    selector: 'lista-cadena',
    templateUrl: 'app/Cadenas/CadenaLista.component.html',
    providers: [CadenasService],
})
export class CadenaListaComponent implements OnInit {
    cadenas: Cadena[];
    constructor(private lasCadenas: CadenasService) {
    }

    ngOnInit() {
        // this.jugadores = this.losJugadores.getJugadores();
        this.lasCadenas.getCadenas().subscribe((datosCadenas) => this.cadenas = datosCadenas);
    }
}