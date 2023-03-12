using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using IFoundBackend.Model.Enums;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IFoundBackend.ControllerModel
{

    public class PersonForm
    {
        public IFormFile Image { get; set; }

        public string Base64Image { get; set; }
        
        [Required]
        [MinLength(20)]
        public string Description { get; set; }
        [Required]
        [MinLength(20)]
        public string Location { get; set; }

        [Range(3, 100)]
        [Required]
        public int? Age { get; set; }
        [Required]
        public int? UserId { get; set; }
        [Required]
        public GenderType? Gender { get; set; }
        [Required]
        public RelationType? Relation { get; set; }
        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        public TargetType? TargetType{ get; set; }
        public PostStatus PostStatus { get; set; } = PostStatus.Unresolved;

        public string convertToBase64(IFormFile file)
        {
            string encoded = "";

            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    encoded = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                }
            }
            return encoded;
        }

        public async Task<byte[]> convertToImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
