using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.localDatabase
{
    public class FaceIdentity
    {
        public int IdentityId { get; set; }
        public FormFile Image { get; set; }

        //public string groupId { get; set; }
        //public string externalId { get; set; }

    }
}
