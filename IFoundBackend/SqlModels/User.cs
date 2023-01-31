using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cnic { get; set; }
        public string Gender { get; set; }
    }
}
