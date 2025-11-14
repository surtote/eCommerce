import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HotelService } from './HotelService';
import { Hotel } from './Hotel';

@Component({
    selector: 'el-hotel',
    templateUrl: 'app/Hoteles/Hotel.component.html',

})
export class HotelComponent implements OnInit {
    hotel: Hotel;

    constructor(private hotelService: HotelService, private activatedRoute: ActivatedRoute) { }

    ngOnInit() {
        let nomHotel = this.activatedRoute.snapshot.params['Nombre'];
        this.hotelService.getHotelesPorNombre(nomHotel).subscribe((HotelDatos) => this.hotel = HotelDatos);
    }
}