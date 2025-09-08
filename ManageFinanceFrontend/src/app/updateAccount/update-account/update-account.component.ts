import {
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
  signal,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FinanceAccount } from '../../models/financeAccount.model';
import { FinanceAccountService } from '../../services/api.FinanceAccountService';
import { TokenService } from '../../services/token.service';
import { UpdateAccountSchema } from '../../schemas/financeAccount.schema';

@Component({
  selector: 'app-update-account',
  imports: [ReactiveFormsModule],
  templateUrl: './update-account.component.html',
  styleUrl: './update-account.component.css',
})
export class UpdateAccountComponent {
  // --------------------------------------------------------------------------
  @Input() accountId!: string;
  // --------------------------------------------------------------------------
  @Output() cancel = new EventEmitter<void>();
  @Output() accountUpdated = new EventEmitter<FinanceAccount>();
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Track the state of the form to create a new account:
  isSubmitted = signal<boolean>(false);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Dependencies:
  private accountService = inject(FinanceAccountService);
  private tokenService = inject(TokenService);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  onCancel() {
    this.cancel.emit();
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Create a Formgrp to handle grp of inputs:
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
        Validators.pattern(/[A-Za-z]+$/),
      ],
    }),
  });
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  onSubmit() {
    if (!this.accountId) {
      return;
    }

    if (this.form.invalid) {
      return;
    }

    const userId = this.tokenService.getUserIdFromToken();
    if (!userId) {
      console.warn('No user ID found in the token.');
      return;
    }

    const { AccountName, AccountType } = this.form.value;
    const accountData: UpdateAccountSchema = {
      UserId: userId,
      AccountName: AccountName ?? '',
      AccountType: AccountType ?? '',
    };

    console.log(AccountName);
    console.log(AccountType);

    this.accountService
      .handleUpdateFinanceAccount(accountData, this.accountId)
      .subscribe({
        next: () => {
          console.log(`Account ${this.accountId} updated successfully.`);
          this.isSubmitted.set(true);
          this.accountUpdated.emit();
          this.cancel.emit();
        },
        error: (err) => {
          console.error(`Failed to update account ${this.accountId}:`, err);
        },
      });
  }
  // --------------------------------------------------------------------------
}
