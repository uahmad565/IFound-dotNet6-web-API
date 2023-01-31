using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels
{
    public partial class PostPerson
    {
        public PostPerson()
        {
            MxFaceIdentities = new HashSet<MxFaceIdentity>();
        }

        public int PostPersonId { get; set; }
        public int? StatusId { get; set; }
        public int? ImageId { get; set; }
        public DateTime? PostDate { get; set; }
        public int UserId { get; set; }
        public int PersonId { get; set; }

        public virtual Image Image { get; set; }
        public virtual TargetPerson Person { get; set; }
        public virtual PostStatus Status { get; set; }
        public virtual ICollection<MxFaceIdentity> MxFaceIdentities { get; set; }
    }
}
