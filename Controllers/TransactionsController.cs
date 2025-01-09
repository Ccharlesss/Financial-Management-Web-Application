using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }













//=============================================================================================================================
//                                              PURPOSE: RETRIEVE ALL TRANSACTIONS
        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {   // 1) Return from the DB a list of all transactions:
            return await _context.Transactions.ToListAsync();
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
        {   // 1) Retrieve the Account where the transaction belongs to:
            var retrievedAccount = await _context.Accounts.FindAsync(transaction.Id);
            if(retrievedAccount==null)
            {
                return NotFound("Account not found");
            }

            // 2) Set the navigation's property:
            transaction.FinanceAccount = retrievedAccount;
            // 3) Create the transaction related to the account:
            _context.Transactions.Add(transaction);

            // 4) Attempt to save the changes made to the DB:
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
