"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var EquipoService_1 = require("./EquipoService");
var EquipoListaComponent = /** @class */ (function () {
    function EquipoListaComponent(losEquipos) {
        this.losEquipos = losEquipos;
    }
    ;
    EquipoListaComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.losEquipos.getEquipo().subscribe(function (datosEquipos) { return _this.equipos = datosEquipos; });
    };
    EquipoListaComponent = __decorate([
        core_1.Component({
            selector: 'lista-equipos',
            templateUrl: 'app/Equipo/EquipoLista.component.html'
        }),
        __metadata("design:paramtypes", [EquipoService_1.EquipoService])
    ], EquipoListaComponent);
    return EquipoListaComponent;
}());
exports.EquipoListaComponent = EquipoListaComponent;
//# sourceMappingURL=EquipoLista.component.js.map