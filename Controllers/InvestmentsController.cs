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
    public class InvestmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvestmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Investments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Investment>>> GetInvestments()
        {
            return await _context.Investments.ToListAsync();
        }

        // GET: api/Investments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Investment>> GetInvestment(string id)
        {
            var investment = await _context.Investments.FindAsync(id);

            if (investment == null)
            {
                return NotFound();
            }

            return investment;
        }

        // PUT: api/Investments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment(string id, Investment investment)
        {
            if (id != investment.Id)
            {
                return BadRequest();
            }

            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // POST: api/Investments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Investment>> PostInvestment(Investment investment)
        {
            _context.Investments.Add(investment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InvestmentExists(investment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetInvestment", new { id = investment.Id }, investment);
        }

        // DELETE: api/Investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(string id)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestmentExists(string id)
        {
            return _context.Investments.Any(e => e.Id == id);
        }
    }
}
