using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFoundBackend.Model.Abstracts;

namespace IFoundBackend.Model.Posts
{
    public class FoundPost : Post
    {
        public List<SecurityQuestion> listQuestions { get; set; }
        

        public FoundPost()
        {

        }
    }
}
