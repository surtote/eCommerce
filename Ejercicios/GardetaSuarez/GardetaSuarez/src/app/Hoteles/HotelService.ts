import { Injectable } from '@angular/core';
import { Hotel } from './Hotel';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class HotelService {
    constructor(private _http: Http) {

    }
    getHoteles(): Observable<Hotel[]> {
        return this._http.get("https://localhost:44371/api/hotel")
            .map((response: Response) => <Hotel[]>response.json())
    }
    getHotelesPorNombre(Nombre: string): Observable<Hotel> {
        return this._http.get("https://localhost:44371/api/hotel/" + Nombre)
            .map((response: Response) => <Hotel>response.json())
    }
}