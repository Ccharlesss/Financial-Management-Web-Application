using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;

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

        // GET: api/FinanceAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinanceAccount>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

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

        // PUT: api/FinanceAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFinanceAccount(string id, FinanceAccount financeAccount)
        {
            if (id != financeAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(financeAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinanceAccountExists(id))
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

        // POST: api/FinanceAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FinanceAccount>> PostFinanceAccount(FinanceAccount financeAccount)
        {
            _context.Accounts.Add(financeAccount);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FinanceAccountExists(financeAccount.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFinanceAccount", new { id = financeAccount.Id }, financeAccount);
        }

        // DELETE: api/FinanceAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinanceAccount(string id)
        {
            var financeAccount = await _context.Accounts.FindAsync(id);
            if (financeAccount == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(financeAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FinanceAccountExists(string id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
