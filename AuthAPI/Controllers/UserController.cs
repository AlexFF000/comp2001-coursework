using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthAPI.Models;

namespace AuthAPI.Controllers.api
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataAccess Database;

        public UserController(DataAccess context)
        {
            Database = context;
        }

        // GET: api/User
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<User>>> Get(User user)
        {
            bool success = await getValidation(user);
            if (success)
            {
                // Generate Jwt token, and send it back to the client in the Authorization header of the response
                int userId = await Database.GetUserId(user);
                string token = GenerateToken(userId);
                HttpContext.Response.Headers.Add("Authorization", token);

            }
            Dictionary<string, bool> body = new Dictionary<string, bool>()
            {
                {"verified", success }
            };
            // Return status code 200 and {'verified': true} if the credentials match or {'verified': false} if not
            return Ok(body);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, User user)
        {
            // PUT request (update existing user details)
            if (HttpContext.User.Claims.FirstOrDefault(name => name.Type == "userId").Value == id.ToString())
            {
                // Only allow the user to be modified if the request contains an authenticated jwt token with the id of the user to be modified
                await Database.Update(user, id);
                return NoContent();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // POST: api/User
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            // POST request (create new user)
            string codeString;
            register(user, out codeString);
            int code = Convert.ToInt32(codeString);
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
        public async Task<ActionResult<User>> Delete(int id)
        {
            // DELETE request
            if (HttpContext.User.Claims.FirstOrDefault(name => name.Type == "userId").Value == id.ToString())
            {
                // Only allow the user to be deleted if the request contains an authenticated jwt token with the id of the user to be deleted
                await Database.Delete(id);
                return NoContent();
            }
            {
                return Unauthorized();
            }
        }     

        [NonAction]
        private async Task<bool> getValidation(User details)
        {
            return await Database.Validate(details);
        }

        [NonAction]
        private void register(User details, out string responseCode)
        {
            Database.Register(details, out responseCode);
        }

        [NonAction]
        private string GenerateToken(int userId)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.PrivateKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Store the id of the logged in user in the token payload
            Claim[] details = new Claim[]
            {
                new Claim("userId", userId.ToString())
            };
            // Create a token that expires after 15 minutes
            JwtSecurityToken token = new JwtSecurityToken(expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials, claims: details);
            // Return the token in the correct string format
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
