using System.ComponentModel.DataAnnotations;

namespace MXFaceAPIOneToNCall.Model.Group
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; }
    }
}
