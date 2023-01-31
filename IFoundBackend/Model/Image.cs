using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.Model
{
    public class Image
    {
        public int ImageId { get; set; }
        public byte[] ImageFile { get; set; }
    }
}
