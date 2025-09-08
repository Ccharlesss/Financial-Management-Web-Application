import { Component } from '@angular/core';
import { AccountsComponent } from '../../accounts/accounts/accounts.component';
// import { RouterOutlet } from '../../../../node_modules/@angular/router/router_module.d-Bx9ArA6K';
import { RouterModule } from '@angular/router';

// @Component({
//   selector: 'app-dashboard',
//   imports: [AccountsComponent, RouterOutlet],
//   templateUrl: './dashboard.component.html',
//   styleUrl: './dashboard.component.css',
// })
// export class DashboardComponent {}

@Component({
  selector: 'app-dashboard',
  imports: [RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {}
