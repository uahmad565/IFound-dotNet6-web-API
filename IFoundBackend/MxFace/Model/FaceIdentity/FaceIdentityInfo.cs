using MXFaceAPIOneToNCall.Model.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class FaceIdentityInfo : BaseResponse
    {
        public int? FaceIdentityId { get; set; }
        public List<FaceInfo> Faces { get; set; }
        public List<GroupResponse> Groups { get; set; }
        // public string encoded_image { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}
