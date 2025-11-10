import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CadenasService } from './CadenaService';
import { Cadena } from './Cadena';

@Component({
    selector: 'la-cadena',
    templateUrl: 'app/Cadena/Cadena.component.html',

})
export class CadenaComponent implements OnInit {
    cadena: Cadena;

    constructor(private cadenaService: CadenasService, private activatedRoute: ActivatedRoute) { }

    ngOnInit() {
        let nomCadena = this.activatedRoute.snapshot.params['nombre'];
        this.cadenaService.getCadenasPorNombre(nomCadena).subscribe((cadenaDatos) => this.cadena = cadenaDatos);
    }
}