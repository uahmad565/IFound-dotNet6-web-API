using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class Target
    {
        public Target()
        {
            TargetPeople = new HashSet<TargetPerson>();
        }

        public int TargetId { get; set; }
        public string TargetName { get; set; }

        public virtual ICollection<TargetPerson> TargetPeople { get; set; }
    }
}
