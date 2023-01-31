using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class Image
    {
        public Image()
        {
            PostPeople = new HashSet<PostPerson>();
        }

        public int ImageId { get; set; }
        public byte[] Pic { get; set; }

        public virtual ICollection<PostPerson> PostPeople { get; set; }
    }
}
