using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels;

public partial class PostStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; }

    public virtual ICollection<PostPerson> PostPeople { get; } = new List<PostPerson>();
}
