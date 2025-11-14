export interface IHotel {
    ID: number;
    Nombre: string;
    Ciudad: string;
    Direccion: string;

}

export class Hotel implements IHotel {

    constructor(public ID: number, public Nombre: string, public Ciudad: string, public Direccion: string) {

    }
}