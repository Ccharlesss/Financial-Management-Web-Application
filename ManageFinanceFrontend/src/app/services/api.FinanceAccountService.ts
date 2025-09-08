import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import {
  CreateAccountSchema,
  UpdateAccountSchema,
} from '../schemas/financeAccount.schema';
import { FinanceAccount } from '../models/financeAccount.model';

@Injectable({
  providedIn: 'root',
})
export class FinanceAccountService {
  // --------------------------------------------------------------------------
  // Dependencies:
  private httpClient = inject(HttpClient);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Endpoints for backend requests for FinanceAccount controller:
  private readonly AccountUrl = 'http://localhost:8088/api/financeAccounts';

  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleCreateFinanceAccount(data: CreateAccountSchema) {
    const body = {
      AccountName: data.AccountName,
      AccountType: data.AccountType,
      UserId: data.UserId,
    };
    return this.httpClient.post(this.AccountUrl, body);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleFetchFinanceAccount(userId: string) {
    return this.httpClient.get<FinanceAccount[]>(
      `${this.AccountUrl}/user/${userId}`
    );
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleRemoveFinanceAccount(accountId: string) {
    return this.httpClient.delete(`${this.AccountUrl}/${accountId}`);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleUpdateFinanceAccount(data: UpdateAccountSchema, accountId: string) {
    const body = {
      AccountName: data.AccountName,
      AccountType: data.AccountType,
      UserId: data.UserId,
    };
    return this.httpClient.put(`${this.AccountUrl}/${accountId}`, body);
  }
  // --------------------------------------------------------------------------
}
