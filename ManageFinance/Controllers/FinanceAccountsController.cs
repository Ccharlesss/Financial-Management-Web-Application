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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<FinanceAccount>>> GetAccounts()
        // {
        //     return await _context.Accounts.ToListAsync();
        // }
        // [HttpGet("{id}")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FinanceAccount>>> GetUserFinanceAccounts(string userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
            
            if(accounts == null || !accounts.Any())
            {
                return NotFound("No accounts found for this user.");
            }
            return Ok(accounts);
        }
//=============================================================================================================================







//=============================================================================================================================
//                                              PURPOSE: GET A FINANCIAL ACCOUNT
        // GET: api/FinanceAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinanceAccount>> GetFinanceAccount(string id)
        {   // 1) Assess if the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var financeAccount = await _context.Accounts.FindAsync(id);

            if (financeAccount == null)
            {
                return NotFound();
            }

            return Ok(financeAccount);
        }
//=============================================================================================================================











//=============================================================================================================================
//                                              PURPOSE: UPDATE A FINANCIAL ACCOUNT
        // PUT: api/FinanceAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFinanceAccount(string id, FinanceAccount financeAccount)
        {   // 1) Assess if the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            // 2) Retrieved the Finance Account with the id:
            var retrievedAccount = await _context.Accounts.FindAsync(id);
            if(retrievedAccount==null)
            {
                return NotFound("Financial Account not found.");
            }

            // 3) Update the fields of the account:
            retrievedAccount.AccountName = financeAccount.AccountName;
            retrievedAccount.AccountType = financeAccount.AccountType;
            // 4) Explicitly change the state of the entry to modified to indicated to EFcore to update the entry in DB:
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
        {   // 1) Assess if the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated");
            }
            // 2) Retrieve the user from the DB:
            var user = await _context.Users.FindAsync(userId);
            if(user==null)
            {
                return NotFound("User not found.");
            }

            // 3) Set the user's navigation property:
            financeAccount.User = user;
            financeAccount.Balance = 0.00M;
            // 4) Create the Financial account:
            _context.Accounts.Add(financeAccount);

            // 5) Attempt to save the entry into the DB:
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
        public async Task<IActionResult> DeleteFinanceAccount(string id)
        {   // 1) Assess if the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            // 2) Retrieve the Financial account from the DB:
            var financeAccount = await _context.Accounts.FindAsync(id);
            if(financeAccount == null)
            {
                return NotFound("Financial Account not found.");
            }
            
            // 3) Update the state of the entry to indicate EFcore to remove the account:
            _context.Accounts.Remove(financeAccount);
            // 4) Save the changes made to the DB and remove the entry:
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
