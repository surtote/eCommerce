import { Injectable } from '@angular/core';
import { Cadena } from './Cadena';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class CadenasService {
    constructor(private _http: Http) {

    }
    getCadenas(): Observable<Cadena[]> {
        return this._http.get("https://localhost:44371/api/cadena")
            .map((response: Response) => <Cadena[]>response.json())
    }
    
    getCadenasPorNombre(nombre: string): Observable<Cadena> {
        return this._http.get("https://localhost:44371/api/cadena/" + nombre)
            .map((response: Response) => <Cadena>response.json())
    }
}