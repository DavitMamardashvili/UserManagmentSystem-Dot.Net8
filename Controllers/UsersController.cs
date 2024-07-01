using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagmentSystem_Dot.Net8.Controllers.Entityes;
using UserManagmentSystem_Dot.Net8.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagmentSystem_Dot.Net8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _dataContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);

            if (user == null)
                return NotFound("User Not Found");

            return Ok(user);
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<User>> AddUser(User newUser)
        {
            _dataContext.Users.Add(newUser);
            await _dataContext.SaveChangesAsync();

            return Ok(newUser);
        }

        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest("User Id mismatch");

            var dbUser = await _dataContext.Users.FindAsync(id);
            if (dbUser == null)
                return NotFound("User Not Found");

            dbUser.Status = user.Status;
            dbUser.Roles = user.Roles;
            dbUser.LastName = user.LastName;
            dbUser.FirstName = user.FirstName;
            dbUser.Email = user.Email;

            await _dataContext.SaveChangesAsync();

            return Ok(true);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var dbUser = await _dataContext.Users.FindAsync(id);

            if (dbUser == null)
                return NotFound("User Not Found");

            _dataContext.Users.Remove(dbUser);
            await _dataContext.SaveChangesAsync();

            return Ok(true);
        }
    }
}


