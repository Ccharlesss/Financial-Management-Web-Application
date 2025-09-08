using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;
using ManageFinance.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Added for Dependency injection:
        private readonly IFinanceAccountService _financeAccountService;

        public TransactionsController(ApplicationDbContext context, IFinanceAccountService financeAccountService)
        {
            _context = context;
            _financeAccountService = financeAccountService;
        }













//=============================================================================================================================
//                                              PURPOSE: RETRIEVE ALL TRANSACTIONS
        // GET: api/Transactions
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(string accountId)
        {   
            var transactions = await _context.Transactions
                .Where(t => t.FinanceAccountId == accountId)
                .ToListAsync();

            if(transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found with this account Id.");
            }

            // 1) Return from the DB a list of all transactions:
            return Ok(transactions);
        }
//=============================================================================================================================




//=============================================================================================================================
//                                              PURPOSE: GET A TRANSACTION
        // GET: api/Transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(string id)
        {   // 1) Attempt to retrieve the transaction based on the id:
            var transaction = await _context.Transactions.FindAsync(id);
            if(transaction==null)
            {
                return NotFound("Transaction not found.");
            }
            return transaction;

        }
//=============================================================================================================================














//=============================================================================================================================
//                                              PURPOSE: UPDATE A TRANSACTION:
        // PUT: api/Transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(string id, Transaction transaction)
        {   // 1) Attempt to retrieve the transaction we are trying to modify:
            var retrievedTransaction = await _context.Transactions.FindAsync(id);
            if(retrievedTransaction==null)
            {
                return NotFound("Transaction not found.");
            }

            // 2) Update the fields of the retrieved transaction:
            retrievedTransaction.Description = transaction.Description;
            retrievedTransaction.Amount = transaction.Amount;
            retrievedTransaction.Date = transaction.Date;
            retrievedTransaction.IsExpense = transaction.IsExpense;
            // 3) Indicate EFcore that the state of the entry is modified: 
            _context.Entry(retrievedTransaction).State = EntityState.Modified;

            // 4) Attempt to retrieve the Account where the transaction lies:
            var retrievedAccount = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == transaction.FinanceAccountId);
            if(retrievedAccount==null)
            {
                return NotFound("Account not found.");
            }

            // 5) Update the balance of the Finance Account:
            retrievedAccount.Balance = _financeAccountService.ComputeBalance(retrievedAccount);

            // 4) Attempt to change the changes made to the DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
//=============================================================================================================================














//=============================================================================================================================
//                                              PURPOSE: CREATE A TRANSACTION
        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {   // 1) Assess the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // 2) Retrieve the Account where the transaction belongs to:
            var retrievedAccount = await _context.Accounts
                // Ensures when retrieve account => all transactions associated with account are also loded
                .Include(a => a.Transactions)
                // FindAsync used to retrieve entity based on PK => not efficient for related entities
                // More efficient other way as it uses a Join thus more efficient
                // one database query is made, fetching both the account and its transactions.
                .FirstOrDefaultAsync(a => a.Id == transaction.FinanceAccountId);

            if(retrievedAccount==null)
            {
                return NotFound("Account not found");
            }

            // 3) Set the navigation's property:
            transaction.FinanceAccount = retrievedAccount;
            // 4) Create the transaction related to the account:
            _context.Transactions.Add(transaction);
            // 5) Update the balance of the finance Account:
            retrievedAccount.Balance = _financeAccountService.ComputeBalance(retrievedAccount);


            // 6) Attempt to save the changes made to the DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(TransactionExists(transaction.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetTransaction", new {id = transaction.Id}, transaction);

        }


        //=============================================================================================================================













        //=============================================================================================================================
        //                                              PURPOSE: REMOVE A TRANSACTION



        // DELETE: api/Transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(string id)
        {   // 1) Retrieve the transaction:
            var transaction = await _context.Transactions.FindAsync(id);
            if(transaction==null)
            {
                return NotFound("Transaction not found.");
            }

            // 2) Update the State of the retrieved entry to delete => Indicate EFcore to remove it when savechangesasync:
            _context.Transactions.Remove(transaction);

            // 3) Retrieve the account tied to the transaction to remove:
            var retrievedAccount = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == transaction.FinanceAccountId);
            if(retrievedAccount == null)
            {
                return NotFound("Account not found.");
            }

            // 4) Recompute the balance of the account:
            retrievedAccount.Balance = _financeAccountService.ComputeBalance(retrievedAccount);

            // 3) Save changes made to the DB and remove entry:
            await _context.SaveChangesAsync();
            return NoContent();

        }
//=============================================================================================================================











//=============================================================================================================================
//                                              PURPOSE: ASSESS IF THE TRANSACTION EXIST
        private bool TransactionExists(string id) // Changed from Guid to string
        {
            return _context.Transactions.Any(e => e.Id == id); // Consistent with string Id
        }
    }
//=============================================================================================================================
}
