import { Component } from '@angular/core';

@Component({
    selector: 'my-app',
    template: `
    <div style="padding:10px">
        <ul class="nav nav-tabs">
            <li><a routerLink="Cadena">Cadenas</a></li>
            <li><a routerLink="Hoteles">Hoteles</a></li>
        </ul>
        <router-outlet></router-outlet>
    </div>
`,
})
export class AppComponent  { name = 'Angular'; }
