using System;
using System.Collections.Generic;

#nullable disable

namespace AuthAPI.Models
{
    public partial class User
    {
        public User()
        {
            Passwords = new HashSet<Password>();
            Sessions = new HashSet<Session>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Password> Passwords { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
