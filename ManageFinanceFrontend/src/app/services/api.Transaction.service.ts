import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  TransactionSchema,
  UpdateTransactionSchema,
} from '../schemas/transaction.Schema';
import { Transaction } from '../models/transaction.model';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  // --------------------------------------------------------------------------
  private httpClient = inject(HttpClient);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Endpoints for backend requests for Transaction controller:
  private readonly TransactionUrl = 'http://localhost:8088/api/transactions';
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleCreateTransaction(data: TransactionSchema) {
    const body = {
      Description: data.Description,
      Amount: data.Amount,
      Date: data.Date,
      IsExpense: data.IsExpense,
      FinanceAccountId: data.FinanceAccountId,
    };
    return this.httpClient.post(this.TransactionUrl, body);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleFetchTransaction(accountId: string) {
    return this.httpClient.get<Transaction[]>(
      `${this.TransactionUrl}/account/${accountId}`
    );
  }

  // --------------------------------------------------------------------------
  handleUpdateTransaction(data: UpdateTransactionSchema, TrasactionId: string) {
    const body = {
      Description: data.NewDescription,
      Amount: data.NewAmount,
      Date: data.NewDate,
      IsExpense: data.NewStatus,
    };
    return this.httpClient.put(`${this.TransactionUrl}/${TrasactionId}`, body);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  handleRemoveTransaction(TransactionId: string) {
    return this.httpClient.delete(`${this.TransactionUrl}/${TransactionId}`);
  }
  // --------------------------------------------------------------------------
}
