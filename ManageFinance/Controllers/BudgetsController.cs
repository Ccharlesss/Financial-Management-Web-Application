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
    public class BudgetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

//=============================================================================================================================
//                                              PURPOSE: RETURN ALL BUDGETS AS A LIST
        // GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudgets()
        {
            return await _context.Budgets.ToListAsync();
        }
//=============================================================================================================================







//=============================================================================================================================
//                                              PURPOSE: GET BUDGET BASED ON ID
        // GET: api/Budgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(string id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }
//=============================================================================================================================








//=============================================================================================================================
//                                              PURPOSE: MODIFY BUDGET ENTRY
        // PUT: api/Budgets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBudget(string id, Budget budget)
        {   // 1) Assess whether the budget exist:
            var existingBudget = await _context.Budgets.FindAsync(id);
            // Case where the object doesn't exist => return a 404:
            if(existingBudget==null){
                return NotFound("Budget not found");
            }

            // 3) Update the fields of the retrieved budgets:
            existingBudget.Category = budget.Category;
            existingBudget.Limit = budget.Limit;
            // 4) explicitly tells EF Core to treat existingBudget as a modified entity to know which operations to perform in the DB:
            // When you call await _context.SaveChangesAsync(), EF Core:
            // Identifies that existingBudget is in a "modified" state.
            // Generates an UPDATE SQL query to update the corresponding row in the database
            // Executes that query to save the changes to the database.
            _context.Entry(existingBudget).State = EntityState.Modified;

            // 3) Attempt to save the changes made to the budget:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!BudgetExists(id))
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
//                                              PURPOSE: CREATE A BUDGET
        // POST: api/Budgets
        [HttpPost]
        public async Task<ActionResult<Budget>> PostBudget(Budget budget)
        {
            // 1) Ensures the User with the following id exist in the database:
            var user = await _context.Users.FindAsync(budget.UserId);
            if(user == null){
                return NotFound("user not found.");
            }
            // 2) Set the User navigation property:
            budget.User = user;
            // 3) Add the budget:
            _context.Budgets.Add(budget);
            // 4) Attempt to save the entry into the database:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BudgetExists(budget.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetBudget", new { id = budget.Id }, budget);
        }
//=============================================================================================================================







//=============================================================================================================================
//                                              PURPOSE: REMOVE A BUDGET
        // DELETE: api/Budgets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(string id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
        }


//=============================================================================================================================
        private bool BudgetExists(string id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
//=============================================================================================================================
    }
}
