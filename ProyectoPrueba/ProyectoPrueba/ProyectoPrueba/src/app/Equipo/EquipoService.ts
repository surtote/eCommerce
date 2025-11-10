import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Equipo } from './Equipo';
import 'rxjs/add/operator/map';

@Injectable()
export class EquipoService {

    constructor(private _http: Http) { }

    getEquipo(): Observable<Equipo[]> {
        return this._http.get("https://localhost:44334/api/Equipo").map((response: Response) => <Equipo[]>response.json());
    }
}