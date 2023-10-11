using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace IFoundBackend.Model
{
    public partial class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cnic { get; set; }
        public string Gender { get; set; }
    }
}
