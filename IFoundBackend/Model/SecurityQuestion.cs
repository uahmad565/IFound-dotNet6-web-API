using IFoundBackend.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.Model
{
    public class SecurityQuestion
    {
        public int QuestionId { get; set; }
        public string Question{ get; set; }
        public IAnswer Answer { get; set; }

    }
}
