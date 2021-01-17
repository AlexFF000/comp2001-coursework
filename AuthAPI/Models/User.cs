using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable

namespace AuthAPI.Models
{
    [DataContract (Namespace = "", Name = "user")]
    public partial class User
    {
        public User()
        {
            Passwords = new HashSet<Password>();
            Sessions = new HashSet<Session>();
        }

        public int UserId { get; set; }
        [DataMember(Order = 0, Name = "firstName")]
        public string FirstName { get; set; }
        [DataMember (Order = 1, Name = "lastName")]
        public string LastName { get; set; }
        [DataMember(Order = 2, Name = "email")]
        public string Email { get; set; }
        [DataMember(Order = 3, Name = "password")]
        public string Password { get; set; }

        public virtual ICollection<Password> Passwords { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
