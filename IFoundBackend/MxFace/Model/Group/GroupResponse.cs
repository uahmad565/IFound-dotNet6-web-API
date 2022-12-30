using System;

namespace MXFaceAPIOneToNCall.Model.Group
{
    public class GroupResponse : BaseResponse
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
