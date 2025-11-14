export interface ICadena {
    ID: number;
    Nombre: string;
    DireccionSede: string;
    Ciudad: string;


}

export class Cadena implements ICadena {

    constructor(public ID: number,public Nombre: string, public DireccionSede: string, public Ciudad: string) {

    }
}