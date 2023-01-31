using IFoundBackend.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend.Model.Abstracts
{
    public abstract class Target
    {
        public int TargetId { get; set; }
        public string Location { get; set; }
        public int age { get; set; }
        public GenderType Gender { get; set; }
        public RelationType relation { get; set; }
        public TargetType Target_type { get; set; }

        public string Description { get; set; }


    }
}
