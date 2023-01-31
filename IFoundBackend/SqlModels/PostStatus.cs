using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class PostStatus
    {
        public PostStatus()
        {
            PostPeople = new HashSet<PostPerson>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<PostPerson> PostPeople { get; set; }
    }
}
