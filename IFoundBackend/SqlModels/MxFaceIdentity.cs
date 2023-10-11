using System;
using System.Collections.Generic;

namespace IFoundBackend.SqlModels;

public partial class MxFaceIdentity
{
    public int PostId { get; set; }

    public int FaceIdentityId { get; set; }

    public virtual PostPerson Post { get; set; }
}
