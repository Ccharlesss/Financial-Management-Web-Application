import { Component, OnInit, inject, input, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FinanceAccount } from '../../models/financeAccount.model';
import { FinanceAccountService } from '../../services/api.FinanceAccountService';
import { TokenService } from '../../services/token.service';
import { CommonModule } from '@angular/common';
import { NewAccountComponent } from '../../newAccount/new-account/new-account.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrash, faPenToSquare } from '@fortawesome/free-solid-svg-icons';
import { UpdateAccountComponent } from '../../updateAccount/update-account/update-account.component';
@Component({
  selector: 'app-accounts',
  imports: [
    CommonModule,
    NewAccountComponent,
    UpdateAccountComponent,
    FontAwesomeModule,
  ],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.css',
})
export class AccountsComponent implements OnInit {
  faTrash = faTrash;
  faPenToSquare = faPenToSquare;
  // --------------------------------------------------------------------------
  constructor(private router: Router, private route: ActivatedRoute) {}
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Dependencies:
  private accountService = inject(FinanceAccountService);
  private tokenService = inject(TokenService);
  // --------------------------------------------------------------------------
  // --------------------------------------------------------------------------
  // State for the list of user accounts:
  // userAccounts = input.required<FinanceAccount[]>();
  userAccounts = signal<FinanceAccount[]>([]);
  showNewAccountForm = signal<boolean>(false);
  showUpdateAccountForm = signal<boolean>(false);
  editingAccountId = signal<string | null>(null);
  selectedAccountId = signal<string | null>(null);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  toggleNewAccountForm() {
    this.showNewAccountForm.set(!this.showNewAccountForm());
  }

  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  ngOnInit(): void {
    const userId = this.tokenService.getUserIdFromToken();

    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    console.log('User ID:', userId);

    this.accountService.handleFetchFinanceAccount(userId).subscribe({
      next: (accounts) => {
        console.log('Fetched accounts:', accounts);

        // Optional: Log missing or duplicate IDs
        const seenIds = new Set<string>();
        accounts.forEach((acc, index) => {
          if (!acc.id) {
            console.warn(`Account at index ${index} is missing an Id:`, acc);
          } else if (seenIds.has(acc.id)) {
            console.warn(`Duplicate Id "${acc.id}" at index ${index}`);
          } else {
            seenIds.add(acc.id);
          }
        });

        // Set to signal
        this.userAccounts.set(accounts);
      },
      error: (err) => console.error('Failed to fetch accounts:', err),
    });
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Refetch all accounts when an account is created:
  handleAccountCreated() {
    const userId = this.tokenService.getUserIdFromToken();
    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    this.accountService.handleFetchFinanceAccount(userId).subscribe({
      next: (accounts) => {
        this.userAccounts.set(accounts);
      },
      error: (err) => {
        console.error('Failed to refetch accounts after creation:', err);
      },
    });
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleAccountUpdated() {
    const userId = this.tokenService.getUserIdFromToken();
    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    this.accountService.handleFetchFinanceAccount(userId).subscribe({
      next: (accounts) => {
        this.userAccounts.set(accounts);
      },
      error: (err) => {
        console.error('Failed to refetch accounts after creation:', err);
      },
    });
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  onEdit(accountId: string) {
    this.editingAccountId.set(accountId);
    this.showUpdateAccountForm.set(true);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  onDelete(accountId: string) {
    console.log(accountId);
    this.accountService.handleRemoveFinanceAccount(accountId).subscribe({
      next: () => {
        console.log(`Account ${accountId} deleted successfully.`);
        this.handleAccountCreated();
      },
      error: (err) => {
        console.error(`Failed to delete account ${accountId}:`, err);
      },
    });
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  goToTransactions(accountId: string) {
    this.router.navigate(['/dashboard/transactions', accountId], {
      relativeTo: this.route,
    });
  }
  // --------------------------------------------------------------------------
}
