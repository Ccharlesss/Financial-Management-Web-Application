import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FinanceAccountService } from '../../services/api.FinanceAccountService';
import { TokenService } from '../../services/token.service';
import {
  CreateAccountSchema,
  FinanceAccountSchema,
} from '../../schemas/financeAccount.schema';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FinanceAccount } from '../../models/financeAccount.model';

@Component({
  selector: 'app-new-account',
  imports: [ReactiveFormsModule],
  templateUrl: './new-account.component.html',
  styleUrl: './new-account.component.css',
})
export class NewAccountComponent {
  // --------------------------------------------------------------------------
  @Output() cancel = new EventEmitter<void>();
  @Output() accountCreated = new EventEmitter<FinanceAccount>();
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Track the state of the form to create a new account:
  isSubmitted = signal<boolean>(false);

  // --------------------------------------------------------------------------
  // Dependencies:
  private accountService = inject(FinanceAccountService);
  private tokenService = inject(TokenService);

  // --------------------------------------------------------------------------
  onCancel() {
    this.cancel.emit();
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Create a FormGroup to handle grp of inputs:
  form = new FormGroup({
    AccountName: new FormControl('', {
      validators: [
        Validators.required,
        Validators.minLength(4),
        Validators.pattern(/^[A-Za-z]+$/),
      ],
    }),

    AccountType: new FormControl('', {
      validators: [
        Validators.required,
        Validators.minLength(4),
        Validators.pattern(/^[A-Za-z]+$/),
      ],
    }),
  });
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Handles the form submission to create a new finance account:
  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const userId = this.tokenService.getUserIdFromToken();
    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    const { AccountName, AccountType } = this.form.value;
    const accountData: CreateAccountSchema = {
      AccountName: AccountName ?? '',
      AccountType: AccountType ?? '',
      UserId: userId,
    };

    console.log('UserId:', userId);

    this.accountService.handleCreateFinanceAccount(accountData).subscribe({
      next: () => {
        this.isSubmitted.set(true);
        // Emit the event to the parent component to notify them:
        this.accountCreated.emit();
        this.cancel.emit();
      },
      error: (err) => {
        console.error('Account creation failed:', err);
      },
    });
  }
}
