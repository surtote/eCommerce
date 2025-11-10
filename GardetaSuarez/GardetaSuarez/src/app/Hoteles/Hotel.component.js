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
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var HotelService_1 = require("./HotelService");
var HotelComponent = (function () {
    function HotelComponent(hotelService, activatedRoute) {
        this.hotelService = hotelService;
        this.activatedRoute = activatedRoute;
    }
    HotelComponent.prototype.ngOnInit = function () {
        var _this = this;
        var nomHotel = this.activatedRoute.snapshot.params['Nombre'];
        this.hotelService.getHotelesPorNombre(nomHotel).subscribe(function (HotelDatos) { return _this.hotel = HotelDatos; });
    };
    return HotelComponent;
}());
HotelComponent = __decorate([
    core_1.Component({
        selector: 'el-hotel',
        templateUrl: 'app/Hoteles/Hotel.component.html',
    }),
    __metadata("design:paramtypes", [HotelService_1.HotelService, router_1.ActivatedRoute])
], HotelComponent);
exports.HotelComponent = HotelComponent;
//# sourceMappingURL=Hotel.component.js.map