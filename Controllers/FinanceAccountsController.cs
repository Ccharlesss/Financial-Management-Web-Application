using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinanceAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

//=============================================================================================================================
//                                              PURPOSE: GET ALL FINANCIAL ACCOUNTS
        // GET: api/FinanceAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinanceAccount>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }
//=============================================================================================================================







//=============================================================================================================================
//                                              PURPOSE: GET A FINANCIAL ACCOUNT
        // GET: api/FinanceAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinanceAccount>> GetFinanceAccount(string id)
        {
            var financeAccount = await _context.Accounts.FindAsync(id);

            if (financeAccount == null)
            {
                return NotFound();
            }

            return financeAccount;
        }
//=============================================================================================================================











//=============================================================================================================================
//                                              PURPOSE: UPDATE A FINANCIAL ACCOUNT
        // PUT: api/FinanceAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> PutFinanceAccount(string id, FinanceAccount financeAccount)
        {
            // 1) Retrieved the Finance Account with the id:
            var retrievedAccount = await _context.Accounts.FindAsync(id);
            if(retrievedAccount==null)
            {
                return NotFound("Financial Account not found.");
            }

            // 2) Update the fields of the account:
            retrievedAccount.AccountName = financeAccount.AccountName;
            retrievedAccount.AccountType = financeAccount.AccountType;
            // 3) Explicitly change the state of the entry to modified to indicated to EFcore to update the entry in DB:
            _context.Entry(retrievedAccount).State = EntityState.Modified;
            // 5) Try to save the changes made to the Entry in DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!FinanceAccountExists(id))
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
//                                              PURPOSE: CREATE A FINANCIAL ACCOUNT
        // POST: api/FinanceAccounts
        [HttpPost]
        public async Task<ActionResult<FinanceAccount>> PostFinanceAccount(FinanceAccount financeAccount)
        {
            // 1) Retrieve the user from the DB:
            var user = await _context.Users.FindAsync(financeAccount.UserId);
            if(user==null)
            {
                return NotFound("User not found.");
            }

            // 2) Set the user's navigation property:
            financeAccount.User = user;
            // 3) Create the Financial account:
            _context.Accounts.Add(financeAccount);

            // 4) Attempt to save the entry into the DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(FinanceAccountExists(financeAccount.Id))
                {
                    return Conflict();
                }
                else 
                {
                    throw;
                }
            }
            return CreatedAtAction("GetFinanceAccount", new {id = financeAccount.Id}, financeAccount);
        }
//=============================================================================================================================













//=============================================================================================================================
//                                              PURPOSE: REMOVE A FINANCIAL ACCOUNT:
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteFinanceAccount(string id)
        {
            // 1) Retrieve the Financial account from the DB:
            var financeAccount = await _context.Accounts.FindAsync(id);
            if(financeAccount == null)
            {
                return NotFound("Financial Account not found.");
            }
            
            // 2) Update the state of the entry to indicate EFcore to remove the account:
            _context.Accounts.Remove(financeAccount);
            // 3) Save the changes made to the DB and remove the entry:
            await _context.SaveChangesAsync();
            return NoContent();
        }
//=============================================================================================================================









//=============================================================================================================================
//                                              PURPOSE: ASSESS IF A FINANCIAL ACCOUNT EXIST
        private bool FinanceAccountExists(string id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
//=============================================================================================================================

    }



}
