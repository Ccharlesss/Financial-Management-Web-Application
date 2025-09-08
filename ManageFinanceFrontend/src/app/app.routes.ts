import { Routes } from '@angular/router';
import { SignupComponent } from './auth/signup/signup.component';
import { LandingComponent } from './landing/landing/landing.component';
import { LoginComponent } from './auth/login/login.component';
import { DashboardComponent } from './dashboard/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';
import { TransactionsComponent } from './transactions/transactions/transactions.component';
import { AccountsComponent } from './accounts/accounts/accounts.component';

export const routes: Routes = [
  // --------------------------------------------------------------------------
  {
    // Default route: if dont have anything in the url:
    path: '',
    component: LandingComponent,
  },

  // --------------------------------------------------------------------------
  {
    // Registering new users:
    path: 'signup',
    component: SignupComponent,
  },
  // --------------------------------------------------------------------------
  {
    path: 'signin',
    component: LoginComponent,
  },
  // --------------------------------------------------------------------------
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'accounts',
        component: AccountsComponent,
      },
      {
        path: 'transactions/:accountId',
        component: TransactionsComponent,
      },
    ],
  },
];

// --------------------------------------------------------------------------

// --------------------------------------------------------------------------
// {
//   path: '**',
// },
