using AngularAuthAPI.Context;
using AngularAuthAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _authContext;

        public UserController(AppDbContext authContext)
        {
            _authContext = authContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            var user = await _authContext.Users
                    .FirstOrDefaultAsync(x => x.Email == userObj.Email && x.Password == userObj.Password);
            if(user == null)
            {
                return NotFound(new {Message = "Utilisateur inconnu."});
            }
            return Ok(user);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            try
            {
                await _authContext.Users.AddAsync(userObj);
                await _authContext.SaveChangesAsync();
                return Ok(userObj);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("/users")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return _authContext.Users.ToList();
        }

        [HttpGet("/users/{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _authContext.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
