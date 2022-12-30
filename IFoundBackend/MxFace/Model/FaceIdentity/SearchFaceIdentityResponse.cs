using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class SearchFaceIdentityResponse : BaseResponse
    {
        public List<LookupIdentities> SearchedIdentities { get; set; }
    }
    public class LookupIdentities : BaseResponse
    {
        public List<IdentityConfidences> identityConfidences { get; set; }
    }
    public class IdentityConfidences
    {
        public Identity identity { get; set; }
        public double confidence { get; set; }
    }
    public class Identity
    {
        public int IdentityId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public List<int> GroupIds { get; set; }
        public string ExternalId { get; set; }

    }
}
