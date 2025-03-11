using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        {   // 1) Assess the user is authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            // 2) Retrieve the goal from the DB:
            var goal = await _context.Goals.FindAsync(id);
            // Case where the goal id doesn't exist:
            if(goal==null)
            {
                return NotFound("Goal not found.");
            }

            // 2) Return the retrieved goal:
            return Ok(goal);
        }
//=============================================================================================================================










//=============================================================================================================================
//                                              PURPOSE: UPDATE A GOAL
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoal(string id, Goal goal)
        {   // Assess if the user is Authenticated:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Retrieve the goal to modify from the DB:
            var retrievedGoal = await _context.Goals.FindAsync(id);
            if(retrievedGoal == null)
            {
                return NotFound("Goal not found.");
            }

            // Update the fields:
            retrievedGoal.GoalTitle = goal.GoalTitle;
            retrievedGoal.TargetAmount = goal.TargetAmount;
            retrievedGoal.CurrentAmount = goal.CurrentAmount;
            retrievedGoal.TargetDate = goal.TargetDate;
            // 4) Explicitly indicate to EFcore that the fields of the Goal were modified:
            _context.Entry(retrievedGoal).State = EntityState.Modified;

            // 5) Attempt to save the changes made to the entry:
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
        // [HttpPost]
        // public async Task<ActionResult<Goal>> PostGoal(Goal goal)
        // {   // 1) Retrieve the user from DB:
        //     var user = await _context.Users.FindAsync(goal.UserId);
        //     // Case where the user doesn't exist => Return 404 not found:
        //     if(user==null)
        //     {
        //         return NotFound("User not found");
        //     }
        //     // 2) Set the User navigation property:
        //     goal.User = user;
        //     // 3) Create the goal:
        //     _context.Goals.Add(goal);
        //     // 4) Attempt to save the entry into the database:
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch(DbUpdateConcurrencyException)
        //     {
        //         if(GoalExists(goal.Id))
        //         {
        //             return Conflict();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //     return CreatedAtAction("GetGoal", new{id = goal.Id}, goal);
        // }



        [HttpPost]
        public async Task<ActionResult<Goal>> PostGoal(Goal goal)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _context.Users.FindAsync(goal.UserId);
                if (user == null)
                {       
                    return NotFound("User not found");
                }
                goal.User = user;
                _context.Goals.Add(goal);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGoal", new { id = goal.Id }, goal);
            }
            else
            {
            return Unauthorized(); // This should return 401 for unauthenticated users
            }
        }

//=============================================================================================================================








//=============================================================================================================================
//                                              PURPOSE: REMOVE A GOAL
        // DELETE: api/Goals/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteGoal(string id)
        // {
        //     // 1) Retrieve the goal from the DB:
        //     var goal = await _context.Goals.FindAsync(id);
        //     if(goal==null)
        //     {
        //         return NotFound("Goal not found.");
        //     }

        //     // 2) Indicate to EFCore the state for this entry is Delete:
        //     _context.Goals.Remove(goal);
        //     // 3) Save the changes made and remove goal => Delete the entry:
        //     await _context.SaveChangesAsync();
        //     return NoContent();
        // }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(string id)
        {
            // 1) Get the authenticated user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // 2) Retrieve the goal from the DB:
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal == null)
            {
                return NotFound("Goal not found.");
            }

            // 3) Delete the goal:
            _context.Goals.Remove(goal);
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
