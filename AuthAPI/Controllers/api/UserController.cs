using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        // Details for hashing the passwords
        private static string Salt = "baxScwIOxIX3/XadCtwdMg==";
        private static int HashIterations = 10000;
        private static int HashLength = 32;
        private readonly COMP2001_ARedmondContext _context;

        public UserController(COMP2001_ARedmondContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Login(User user)
        {
            bool success = await ValidateUser(user);
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
        public async Task<IActionResult> Edit(int id, User user)
        {
            // PUT request (update existing user details)
            await UpdateUser(id, user);
            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            // POST request (create new user)
            // Call Register stored procedure
            int code = await RegisterUser(user);
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
            await RemoveUser(id);
            return NoContent();
        }

        [NonAction]
        public async Task<bool> ValidateUser(User details)
        {
            // Use database stored procedure to check an email and password
            SqlParameter response = new SqlParameter("@Response", SqlDbType.Int);
            response.Direction = ParameterDirection.Output;
            // Use ExecuteSqlRawAsync and await the result to allow this thread to be returned to the pool while waiting for the database response
            await _context.Database.ExecuteSqlRawAsync("EXEC @Response = ValidateUser @Email, @Password",
                response,
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", HashPassword(details.Password)));
            // Return true if response is 1 (meaning validation was successful) and false otherwise
            return (int)response.Value == 1;
        }

        [NonAction]
        public async Task UpdateUser(int idToUpdate, User details)
        {
            // Use database stored procedure to change user details
            // Must replace any empty strings with null, as the stored procedure will not update fields with null values supplied
            await _context.Database.ExecuteSqlRawAsync("EXEC UpdateUser @FirstName, @LastName, @Email, @Password, @id",
                new SqlParameter("@FirstName", ReturnDbNullIfEmpty(details.FirstName)),
                new SqlParameter("@LastName", ReturnDbNullIfEmpty(details.LastName)),
                new SqlParameter("@Email", ReturnDbNullIfEmpty(details.Email)),
                new SqlParameter("@Password", ReturnDbNullIfEmpty(HashPassword(details.Password))),
                new SqlParameter("@id", idToUpdate));
        }

        [NonAction]
        public async Task<int> RegisterUser(User details)
        {
            // Try to create new user, and return response code
            // Pass back user id by changing userId property of details

            SqlParameter response = new SqlParameter("@ResponseMessage", SqlDbType.VarChar, 10);
            response.Direction = ParameterDirection.Output;
            await _context.Database.ExecuteSqlRawAsync("EXEC Register @FirstName, @LastName, @Email, @Password, @ResponseMessage OUTPUT",
                new SqlParameter("@FirstName", details.FirstName),
                new SqlParameter("@LastName", details.LastName),
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", HashPassword(details.Password)),
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

        [NonAction]
        public async Task RemoveUser(int idToRemove)
        {
            // Use DeleteUser stored procedure to delete the user with the given Id
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteUser @id",
                new SqlParameter("@id", idToRemove));
        }

        [NonAction]
        public object ReturnDbNullIfEmpty(string inputString)
        {
            // Return a DbNull object if the string is empty, and return the string if not empty
            return String.IsNullOrWhiteSpace(inputString) ? (object)DBNull.Value : (object)inputString;
        }

        [NonAction]
        public string HashPassword(string password)
        {
            // Hash a password with ASP.NET's inbuilt PBKDF2 implementation
            // Do not hash and simply return null if a blank string was provided (allowing ReturnDbNullIfEmpty to work)
            if (String.IsNullOrWhiteSpace(password)) return null;
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                Convert.FromBase64String(Salt),
                KeyDerivationPrf.HMACSHA1,
                HashIterations,
                HashLength));
        }
    }
}
