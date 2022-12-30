using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class UpdateGroupRequest
    {
        public List<int> AddGroupIds { get; set; }
        public List<int> DeleteGroupIds { get; set; }
    }
}
