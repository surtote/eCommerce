import { Component, OnInit } from '@angular/core';
import { Equipo } from './Equipo';
import { EquipoService } from './EquipoService';
@Component({
    selector: 'lista-equipos',
    templateUrl: 'app/Equipo/EquipoLista.component.html'
})

export class EquipoListaComponent implements OnInit{
    equipos: Equipo[];
    constructor(private losEquipos: EquipoService) {
    };
    ngOnInit() {
        this.losEquipos.getEquipo().subscribe(datosEquipos => this.equipos = datosEquipos);
    }
}