﻿using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class TargetPerson
    {
        public TargetPerson()
        {
            PostPeople = new HashSet<PostPerson>();
        }

        public int PersonId { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int TargetId { get; set; }
        public string Name { get; set; }

        public virtual Target Target { get; set; }
        public virtual ICollection<PostPerson> PostPeople { get; set; }
    }
}
