using IFoundBackend.SqlModels;
using System.Collections.Generic;
using System;

namespace IFoundBackend.Areas.ToDTOs
{
    
    public class ImageDto
    {
        public int ImageId { get; set; }
        public string Base64String { get; set; }
    }

    public partial class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cnic { get; set; }
        public string Gender { get; set; }
    }

    public partial class TargetPersonDto
    {
        public int PersonId { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int TargetId { get; set; }
        public string Name { get; set; }

    }
    public class PostDto
    {
        public int MxIdentityFaceID { get; set; }
        public int PostPersonId { get; set; }
        public int StatusId { get; set; }
        public DateTime? PostDate { get; set; }
        public ImageDto ImageDto { get; set; }
        public int UserID { get; set; }
        public TargetPersonDto TargetPersonDto { get; set; }
    }
}
