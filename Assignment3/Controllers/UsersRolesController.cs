using Assignment3.Data;
using Assignment3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/UsersRoles/assignrole
        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {model.UserId} not found.");
            }

            // Find the role
            var role = await _context.Roles.FindAsync(model.RoleId);
            if (role == null)
            {
                return NotFound($"Role with ID {model.RoleId} not found.");
            }

            // Check if the user already has the role
            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == model.UserId && ur.RoleId == model.RoleId);

            if (existingUserRole != null)
            {
                return Conflict($"User with ID {model.UserId} already has the role {role.RoleName}.");
            }

            // Assign the role to the user
            var userRole = new AssignRoleModel
            {
                UserId = model.UserId,
                RoleId = model.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok($"Role {role.RoleName} assigned to User {user.Name} successfully.");
        }
    }
}
