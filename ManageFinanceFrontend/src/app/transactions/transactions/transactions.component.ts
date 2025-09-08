import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TokenService } from '../../services/token.service';
import { TransactionService } from '../../services/api.Transaction.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transactions',
  imports: [CommonModule],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.css',
})
export class TransactionsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  accountId!: string;

  // --------------------------------------------------------------------------
  private tokenService = inject(TokenService);
  private transactionService = inject(TransactionService);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  accountTransactions = signal<Transaction[]>([]);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  ngOnInit(): void {
    // Get the userId to assess if the user is authenticated:
    const userId = this.tokenService.getUserIdFromToken();
    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    // Fetch the respective transactions from the accountId:
    this.accountId = this.route.snapshot.paramMap.get('accountId')!;
    console.log('Loaded transactions for account:', this.accountId);
    this.transactionService.handleFetchTransaction(this.accountId).subscribe({
      next: (transactions) => {
        console.log('Fetched transactions:', transactions);
        this.accountTransactions.set(transactions ?? []);
      },

      error: (err) => {
        if (err.status === 404) {
          console.log('No transactions found for this account.');
          this.accountTransactions.set([]); // Set empty array instead of throwing error
        } else {
          console.error('Failed to fetch transactions from the account');
        }
      },
    });
  }
  // --------------------------------------------------------------------------
}
