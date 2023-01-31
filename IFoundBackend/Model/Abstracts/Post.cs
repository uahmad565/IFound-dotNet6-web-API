using IFoundBackend.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.Model.Abstracts
{
    public abstract class Post
    {
        public int PostId { get; set; }
        public Target Target { get; set; }

        public List<Image> ImageList { get; set; }

        public int UserId { get; set; }

    }
}
