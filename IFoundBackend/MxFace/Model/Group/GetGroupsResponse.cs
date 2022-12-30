using System.Collections.Generic;

namespace MXFaceAPIOneToNCall.Model.Group
{
    public class GetGroupsResponse : BaseResponse
    {
        public IEnumerable<GroupResponse> Groups { get; set; }
    }
}
