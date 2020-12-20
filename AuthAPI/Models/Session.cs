using System;
using System.Collections.Generic;

#nullable disable

namespace AuthAPI.Models
{
    public partial class Session
    {
        public int UserId { get; set; }
        public DateTime SessionTime { get; set; }

        public virtual User User { get; set; }
    }
}
