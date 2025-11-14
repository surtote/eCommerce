import { Component, OnInit } from '@angular/core';
import { Hotel } from './Hotel';
import { HotelService } from './HotelService';

@Component({
    selector: 'lista-hoteles',
    templateUrl: 'app/Hoteles/HotelLista.component.html',
    providers: [HotelService],
})
export class HotelListaComponent implements OnInit {
    hoteles: Hotel[];
    constructor(private losHoteles: HotelService) {
    }

    ngOnInit() {
        // this.jugadores = this.losJugadores.getJugadores();
        this.losHoteles.getHoteles().subscribe((datosHoteles) => this.hoteles = datosHoteles );
    }
}