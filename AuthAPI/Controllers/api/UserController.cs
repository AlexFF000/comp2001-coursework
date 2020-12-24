using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using AuthAPI.Models;

namespace AuthAPI.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly COMP2001_ARedmondContext _context;

        public UserController(COMP2001_ARedmondContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(User user)
        {
            Dictionary<string, bool> body = new Dictionary<string, bool>()
            {
                {"verified", ValidateUser(user) }
            };
            // Return status code 200 and {'verified': true} if the credentials match or {'verified': false} if not
            return Ok(body);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/User
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // POST request (create new user)
            // Call Register stored procedure
            int code = RegisterUser(user);
            // Return status code and (if successful) the new userId
            if (code == 200)
            {
                Dictionary<string, int> body = new Dictionary<string, int>()
                {
                    {"UserID", user.UserId }
                };
                return Ok(body);
            }
            else
            {
                return StatusCode(code);
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public bool ValidateUser(User details)
        {
            // Use database stored procedure to check an email and password
            SqlParameter response = new SqlParameter("@Response", SqlDbType.Int);
            response.Direction = ParameterDirection.Output;
            _context.Database.ExecuteSqlRaw("EXEC @Response = ValidateUser @Email, @Password",
                response,
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", details.Password));
            // Return true if response is 1 (meaning validation was successful) and false otherwise
            return (int)response.Value == 1;
        }

        public int RegisterUser(User details)
        {
            // Try to create new user, and return response code
            // Pass back user id by changing userId property of details

            SqlParameter response = new SqlParameter("@ResponseMessage", SqlDbType.VarChar, 10);
            response.Direction = ParameterDirection.Output;
            _context.Database.ExecuteSqlRaw("EXEC Register @FirstName, @LastName, @Email, @Password, @ResponseMessage OUTPUT",
                new SqlParameter("@FirstName", details.FirstName),
                new SqlParameter("@LastName", details.LastName),
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", details.Password),
                response);
            // Process response
            // The stored procedure separates the response code and user Id with a comma, so split the string on commas
            string responseString = response.Value.ToString();
            string[] responseComponents = responseString.Split(',');
            if (responseComponents[0] == "200")
            {
                // New user was created successfully, so pass back the new user id in the User object
                details.UserId = Convert.ToInt32(responseComponents[1]);
            }
            return Convert.ToInt32(responseComponents[0]);
        }

    }
}
