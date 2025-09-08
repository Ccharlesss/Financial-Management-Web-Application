import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TestimonialsComponent } from '../../testimonials/testimonials/testimonials.component';
import { PricingComponent } from '../../pricing/pricing/pricing.component';
import { FooterComponent } from '../../footer/footer/footer.component';

@Component({
  selector: 'app-landing',
  imports: [
    RouterLink,
    TestimonialsComponent,
    PricingComponent,
    FooterComponent,
  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css',
})
export class LandingComponent {
  // Signal that will track the state of the isAuthenticated property:
  isAuthenticated = input(false);
}
