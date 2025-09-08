import { Component } from '@angular/core';
// import { RouterLink, RouterOutlet } from '@angular/router';
import { RouterOutlet } from '@angular/router';
// import { LandingComponent } from './landing/landing/landing.component';
// import { MenuComponent } from './navigation/menu/menu.component';
// import { PricingComponent } from './pricing/pricing/pricing.component';
// import { TestimonialsComponent } from './testimonials/testimonials/testimonials.component';
// import { FooterComponent } from './footer/footer/footer.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    // RouterLink,
    // LandingComponent,
    // MenuComponent,
    // PricingComponent,
    // TestimonialsComponent,
    // FooterComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'ManageFinanceFrontend';
}
