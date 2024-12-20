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
    public class GoalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoalsController(ApplicationDbContext context)
        {
            _context = context;
        }



//=============================================================================================================================
//                                              PURPOSE: GET ALL GOALS
        // GET: api/Goals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoals()
        {
            return await _context.Goals.ToListAsync();
        }
//=============================================================================================================================











//=============================================================================================================================
//                                              PURPOSE: GET A GOAL
        // GET: api/Goals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Goal>> GetGoal(string id)
        {
            // 1) Retrieve the goal from the DB:
            var goal = await _context.Goals.FindAsync(id);
            // Case where the goal id doesn't exist:
            if(goal==null)
            {
                return NotFound("Goal not found.");
            }

            // 2) Return the retrieved goal:
            return goal;
        }
//=============================================================================================================================










//=============================================================================================================================
//                                              PURPOSE: UPDATE A GOAL
        // PUT: api/Goals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoal(string id, Goal goal)
        {
            // 1) Retrieve the goal with the entered id:
            var retrievedGoal = await _context.Goals.FindAsync(id);
            // Case where the goal with the id doesn't exist:
            if(retrievedGoal == null)
            {
                return NotFound("Goal not found.");
            }

            // 3) Update the fields of the retrieved goal:
            retrievedGoal.GoalTitle = goal.GoalTitle;
            retrievedGoal.TargetAmount = goal.TargetAmount;
            retrievedGoal.CurrentAmount = goal.CurrentAmount;
            retrievedGoal.TargetDate = goal.TargetDate;
            // 4) Explicitly indicate EFCore to change the state of the retrievedGoal as modified:
            _context.Entry(retrievedGoal).State = EntityState.Modified;

            // 5) Save the changes made to the entry:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!GoalExists(id))
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
//                                              PURPOSE: CREATE A GOAL:
        // POST: api/Goals
        [HttpPost]
        public async Task<ActionResult<Goal>> PostGoal(Goal goal)
        {   // 1) Retrieve the user from DB:
            var user = await _context.Users.FindAsync(goal.UserId);
            // Case where the user doesn't exist => Return 404 not found:
            if(user==null)
            {
                return NotFound("User not found");
            }
            // 2) Set the User navigation property:
            goal.User = user;
            // 3) Create the goal:
            _context.Goals.Add(goal);
            // 4) Attempt to save the entry into the database:
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(GoalExists(goal.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetGoal", new{id = goal.Id}, goal);
        }
//=============================================================================================================================








//=============================================================================================================================
//                                              PURPOSE: REMOVE A GOAL
        // DELETE: api/Goals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(string id)
        {
            // 1) Retrieve the goal from the DB:
            var goal = await _context.Goals.FindAsync(id);
            if(goal==null)
            {
                return NotFound("Goal not found.");
            }

            // 2) Indicate to EFCore the state for this entry is Delete:
            _context.Goals.Remove(goal);
            // 3) Save the changes made and remove goal => Delete the entry:
            await _context.SaveChangesAsync();
            return NoContent();
        }
//=============================================================================================================================








//=============================================================================================================================
//                                              PURPOSE: ASSESS IF A GOAL EXIST WITH THIS ID
        private bool GoalExists(string id)
        {
            return _context.Goals.Any(e => e.Id == id);
        }
    }
//=============================================================================================================================
}
