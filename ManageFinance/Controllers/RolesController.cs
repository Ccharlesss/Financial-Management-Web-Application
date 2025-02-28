// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Identity;


// namespace ManageFinance.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController] // Route defined is: /api/Admin
//     //[Authorize(Roles = "Admin")] // Only admins are allowed to access this controller!

//     public class RolesController : ControllerBase
//     {
//         private readonly RoleManager<IdentityRole> _roleManager; // manage user's permissions:

//         private readonly UserManager<IdentityUser> _userManager; // manage the user:

//         private readonly ILogger<RolesController> _logger;


//         public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger<RolesController> logger)
//         {   // Allow the Controller to access both services
//             _roleManager = roleManager;
//             _userManager = userManager;
//             _logger = logger;
//         }





// // ==========================================================================================================
//         [HttpGet] 
//         // PurPose: Retrieves all roles stored in AspNetRole:
//         public IActionResult GetRoles()
//         {   // Retrieves all roles and add them to a list:
//             var roles = _roleManager.Roles.ToList();
//             // Returns HTTP 200 OK w/ list of roles:
//             return Ok(roles); 
//         }
// // ==========================================================================================================







// // ==========================================================================================================
//         [HttpGet("{roleId}")] // Route: /api/Admin/roles/{roleId}
//         // Purpose: Get a specific role from its Id
//         public async Task<IActionResult> GetRole(string roleId)
//         {
//             // Search for a roal w/ specified roleId using _roleManager method
//             var role = await _roleManager.FindByIdAsync(roleId);
//             // Case where role isn't found:
//             if (role == null)
//             {   // Returns HTTP 404 Not Found if no role id was found
//                 _logger.LogWarning($"The role with the following roleId = {roleId} couldn't be found!");
//                 return NotFound("Role not found.");
//             }
//             // Returns HTTP 200 OK if a role id was found
//             return Ok(role);
//         }
// // ==========================================================================================================





// // ==========================================================================================================
// // Purpose: Create a new Role:
//         [HttpPost]
//         public async Task<IActionResult> CreateRole([FromBody] CreateRoleSchema data)
//         {   // Assess if a RoleName was provided:
//             if(data.RoleName == null || string.IsNullOrEmpty(data.RoleName))
//             {
//                 return BadRequest("The 'RoleName' field is required.");
//             }
//             // Initialize a new instance of IdentityRole to create a new Role. + Generate a UUID:
//             var role = new IdentityRole(data.RoleName);
//             // Attempts to create a new role in the AspNetRole:
//             var result = await _roleManager.CreateAsync(role);

//             if(result.Succeeded)
//             {
//                 return Ok("Role created successfully.");
//             }

//             return BadRequest(result.Errors);
//         }
// // ==========================================================================================================





// // ==========================================================================================================
//         [HttpPut]
//         // Purpose: Update a role present in AspNetRole:
//         public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleSchema model)
//         {   // Attempt to retrieve the role to be updated w/ its Id from AspNetRole table:
//             var role = await _roleManager.FindByIdAsync(model.RoleId);
//             // Case where no matching roles could be retrieved:
//             if (role == null)
//             {   // Returns HTTP 404 Not Found if no role could be found:
//                 _logger.LogWarning($"The role with the following roleId = {model.RoleId} couldn't be found!");
//                 return NotFound("Role not found.");
//             }

//             // Update the role's name to its new name:
//             role.Name = model.NewRoleName;
//             // Attempt to commit the change:
//             var result = await _roleManager.UpdateAsync(role);

//             // Case where commit was successfull:
//             if (result.Succeeded)
//             {   // Returns HTTP 200 OK if role was successfully updated
//                 return Ok("Role updated successfully.");
//             }
//             // HTTP 400 Bad Request if the operation failed
//             _logger.LogError($"Operation failed, the role couldn't be updated");
//             return BadRequest(result.Errors);
//         }
// // ==========================================================================================================







// // ==========================================================================================================
//         [HttpDelete]
//         // Purpose: Remove a role from AspNetRole based on its id:
//         public async Task<IActionResult> DeleteRole(string roleId)
//         {   // Attempt to retrieve the role to be removed from AspNetRole:
//             var role = await _roleManager.FindByIdAsync(roleId);
//             // Case where no matching role could be found:
//             if (role == null)
//             {   // Returns HTTP 404 Not Found if no role could be found:
//                 _logger.LogWarning($"The role with the following roleId = {roleId} couldn't be found!");
//                 return NotFound("Role not found.");
//             }
//             // Case where a matching role was found: Attempts to remove it from the table:
//             var result = await _roleManager.DeleteAsync(role);
//             // Case where role was successfully removed:
//             if (result.Succeeded)
//             {   // Returns HTTP 200 OK if the role was successfully removed:
//                 return Ok("Role deleted successfully.");
//             }
//             // HTTP 400 Bad Request if the operation failed:
//             _logger.LogError($"Operation failed! the role with the roleId = {roleId} coudn't get removed");
//             return BadRequest(result.Errors);
//         }
// // ==========================================================================================================








// // ==========================================================================================================
//         [HttpPost("assign-role-to-user")] // Route: /api/Admin/assign-role-to-user
//         // Purpose: Assign a permission to a user:
//         // [FromBody]: Indicates that the value for this parameter should be taken from the request body:
//         public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleSchema model)
//         {   // Retrieve the user from whom the role will be assigned using userId:
//             var user = await _userManager.FindByIdAsync(model.UserId);
//             // Case where no user could be retrieved:
//             if (user == null)
//             {   // Returns HTTP 404 Not Found if no user id is found
//                 _logger.LogWarning($"No user with the userId = {model.UserId} couldn't be found!");
//                 return NotFound("User not found.");
//             }
//             // Case where a user could be retrieved => Attempts to verify if the permission exist:
//             var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

//             // Case where the role doesn't exist in AspNetRole:
//             if (!roleExists)
//             {   // Returns HTTP 404 Not Found if the permission could not be found
//                 _logger.LogWarning($"The role {model.RoleName} couldn't be found!");
//                 return NotFound("Role not found.");
//             }

//             // Attempts to assign the permission to the user:
//             var result = await _userManager.AddToRoleAsync(user, model.RoleName);
//             // Case where role was successfully assigned to the user:
//             if (result.Succeeded)
//             {   // Returns HTTP 200 OK if the role was successfully assigned:
//                 return Ok("Role assigned to user successfully.");
//             }
//             // HTTP 400 Bad Request if the operation failed:
//             _logger.LogError($"Operation failed the role {model.RoleName} couldn't be assigned to the user {model.UserId} ");
//             return BadRequest(result.Errors);
//         }
//     }
// // ==========================================================================================================
// }





using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this if you want to restrict access to admins only
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager; // Manage roles
        private readonly UserManager<AppUser> _userManager; // Manage users
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        // ==========================================================================================================
        [HttpGet]
        // Purpose: Retrieves all roles stored in AspNetRole
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList(); // Retrieves all roles
            return Ok(roles); // Returns HTTP 200 OK with the list of roles
        }
        // ==========================================================================================================

        // ==========================================================================================================
        [HttpGet("{roleId}")] // Route: /api/Roles/{roleId}
        // Purpose: Get a specific role from its Id
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning($"The role with the following roleId = {roleId} couldn't be found!");
                return NotFound("Role not found.");
            }
            return Ok(role); // Returns HTTP 200 OK if a role is found
        }
        // ==========================================================================================================

        // ==========================================================================================================
        // Purpose: Create a new Role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleSchema data)
        {
            if (string.IsNullOrEmpty(data.RoleName)) // Check if RoleName is provided
            {
                return BadRequest("The 'RoleName' field is required.");
            }

            var role = new IdentityRole(data.RoleName); // Initialize a new IdentityRole
            var result = await _roleManager.CreateAsync(role); // Attempt to create the role

            if (result.Succeeded)
            {
                return Ok("Role created successfully.");
            }

            return BadRequest(result.Errors); // Return errors if creation failed
        }
        // ==========================================================================================================

        // ==========================================================================================================
        [HttpPut]
        // Purpose: Update a role present in AspNetRole
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleSchema model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                _logger.LogWarning($"The role with the following roleId = {model.RoleId} couldn't be found!");
                return NotFound("Role not found.");
            }

            role.Name = model.NewRoleName; // Update role's name
            var result = await _roleManager.UpdateAsync(role); // Attempt to commit the change

            if (result.Succeeded)
            {
                return Ok("Role updated successfully.");
            }

            _logger.LogError("Operation failed, the role couldn't be updated");
            return BadRequest(result.Errors); // Return errors if update failed
        }
        // ==========================================================================================================

        // ==========================================================================================================
        [HttpDelete("{roleId}")] // Route: /api/Roles/{roleId}
        // Purpose: Remove a role from AspNetRole based on its id
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning($"The role with the following roleId = {roleId} couldn't be found!");
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role); // Attempt to remove the role
            if (result.Succeeded)
            {
                return Ok("Role deleted successfully.");
            }

            _logger.LogError($"Operation failed! the role with the roleId = {roleId} couldn't be removed");
            return BadRequest(result.Errors); // Return errors if deletion failed
        }
        // ==========================================================================================================

        // ==========================================================================================================
        [HttpPost("assign-role-to-user")] // Route: /api/Roles/assign-role-to-user
        // Purpose: Assign a role to a user
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleSchema model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning($"No user with the userId = {model.UserId} could be found!");
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                _logger.LogWarning($"The role {model.RoleName} couldn't be found!");
                return NotFound("Role not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName); // Attempt to assign the role
            if (result.Succeeded)
            {
                return Ok("Role assigned to user successfully.");
            }

            _logger.LogError($"Operation failed: the role {model.RoleName} couldn't be assigned to the user {model.UserId}");
            return BadRequest(result.Errors); // Return errors if assignment failed
        }
        // ==========================================================================================================
    }
}
