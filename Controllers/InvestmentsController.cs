using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;
using System.Data;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvestmentsController(ApplicationDbContext context)
        {
            _context = context;
        }






//=============================================================================================================================
//                                              PURPOSE: GET ALL INVESTMENTS:
        // GET: api/Investments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Investment>>> GetInvestments()
        {   // 1) Return all Investments present in the DB in a list format:
            return await _context.Investments.ToListAsync();
        }
//=============================================================================================================================









//=============================================================================================================================
//                                              PURPOSE: GET AN INVESTMENT:
        // GET: api/Investments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Investment>> GetInvestment(string id)
        {   // 1) Retrieve the investment from the DB:
            var investment = await _context.Investments.FindAsync(id);
            if(investment==null)
            {
                return NotFound("Investment not found.");
            }
            return investment;
        }
//=============================================================================================================================












//=============================================================================================================================
//                                              PURPOSE: UPDATE AN INVESTMENT:
        // PUT: api/Investments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment(string id, Investment investment)
        {   // 1) Retrieve the Investment:
            var retrievedInvestment = await _context.Investments.FindAsync(id);
            if(retrievedInvestment==null)
            {
                return NotFound("Investment not found.");
            }
            // 2) Update the fields of the retrieved investment:
            retrievedInvestment.AssetName = investment.AssetName;
            retrievedInvestment.AmountInvested = investment.AmountInvested;
            retrievedInvestment.CurrentValue = investment.CurrentValue;
            retrievedInvestment.PurchaseDate = investment.PurchaseDate;
            // 3) Indicate EFcore that the state of the entry is modified:
            _context.Entry(retrievedInvestment).State = EntityState.Modified;

            // 4) Save the changes made to the DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!InvestmentExists(id))
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
//                                              PURPOSE: POST AN INVESTMENT:
        // POST: api/Investments
        [HttpPost]
        public async Task<ActionResult<Investment>> PostInvestment(Investment investment)
        {   // 1) Retrieve the user from DB:
            var user = await _context.Users.FindAsync(investment.UserId);
            if(user==null)
            {
                return NotFound("User not found.");
            }

            // 2) Set the user's navigation property:
            investment.User = user;
            // 3) Create the investment:
            _context.Investments.Add(investment);

            // 4) Attempt to save the entry into the DB:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DBConcurrencyException)
            {
                if(InvestmentExists(investment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetInvestment", new {id = investment.Id}, investment);
        }
//=============================================================================================================================














//=============================================================================================================================
//                                              PURPOSE: DELETE AN INVESTMENT
        // DELETE: api/Investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(string id)
        {   // 1) Retrieve the investment from the DB:
            var investment = await _context.Investments.FindAsync(id);
            if(investment==null)
            {
                return NotFound("Investment Not found.");
            }

            // 2) Update the state of the investment retrieved to Delete to indicate to EFcore to remove it when change is saved to DB:
            _context.Investments.Remove(investment);
            // 3) Save the changes made and remove the investment:
            await _context.SaveChangesAsync();
            return NoContent();
        }
//=============================================================================================================================













//=============================================================================================================================
//                                              PURPOSE: ASSESS IF AN INVESTMENT EXIST
        private bool InvestmentExists(string id)
        {
            return _context.Investments.Any(e => e.Id == id);
        }
    }
//=============================================================================================================================

}
