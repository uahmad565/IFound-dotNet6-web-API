using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class GetFaceResponse : BaseResponse
    {
        public List<FaceInfo> Faces { get; set; }
    }
}
