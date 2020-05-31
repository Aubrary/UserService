using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Entities;
using UserService.Interfaces;
using UserService.Services;

namespace UserService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserLoginService _userLoginService;
        private readonly UserServiceContext _context;

        public UsersController(IUserLoginService userLoginService, UserServiceContext context)
        {
            _userLoginService = userLoginService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _userLoginService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<User> RegisterUser(User model)
        {

            var entity = _userLoginService.MapToEntity(model);
            entity.Role = Role.Subscriber;
            
            _context.Users.Add(entity);
            _context.SaveChanges();

            return Ok(entity);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userLoginService.GetAll();
            
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user =  _userLoginService.GetById(id);

            if (user == null) {
                return NotFound();
            }

            // Only administrators can access other users.
            var currentUserId = Guid.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin)) {
                return Forbid();
            }

            return Ok(user);
        }
    }
}
