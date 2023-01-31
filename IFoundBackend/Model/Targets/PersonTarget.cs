using IFoundBackend.Model.Abstracts;
using IFoundBackend.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.Model.Targets
{
    public class PersonTarget: Target
    {

        public int age { get; set; }
        public GenderType Gender { get; set; }
    }
}
