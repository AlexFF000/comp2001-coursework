using System;
using System.Collections.Generic;

#nullable disable

namespace AuthAPI.Models
{
    public partial class Password
    {
        public int UserId { get; set; }
        public string Password1 { get; set; }
        public DateTime TimeChanged { get; set; }

        public virtual User User { get; set; }
    }
}
