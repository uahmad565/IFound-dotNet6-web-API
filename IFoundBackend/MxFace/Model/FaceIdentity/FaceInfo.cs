using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class FaceInfo : BaseResponse
    {
        public string? FaceId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
