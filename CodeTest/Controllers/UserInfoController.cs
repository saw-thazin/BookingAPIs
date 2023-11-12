using CodeTest.Model;
using CodeTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace CodeTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly BookingSystem _context;

        public UserInfoController(BookingSystem context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> userLogin([FromBody] UserInfo user)
        {
            var dbUser = _context.UserInfo.Where(u => u.Email == user.Email && u.password == user.password).SingleOrDefault();

            if (dbUser == null)
            {
                return BadRequest("Username or password is incorrect. Please try again");
            }

            return Ok(dbUser);
        }


        [HttpPost]
        [Route("register")]

        public async Task<IActionResult> userRgistration([FromBody] UserInfo user)
        {
            var dbUser = _context.UserInfo.Where(u => u.Email == user.Email).FirstOrDefault();
            if(dbUser != null)
            {
                return BadRequest("Email already exists");

            }

            _context.UserInfo.Add(user);
            _context.SaveChangesAsync();

            return Ok("User is successfully created");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfos()
        {
            return await _context.UserInfo.ToListAsync();
        }
     
    }
}
