using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class FaceIdentityResponse : BaseResponse
    {
        public List<FaceIdentityInfo> FaceIdentities { get; set; }
        public int TotalFaceIdentities { get; set; }
    }
}
