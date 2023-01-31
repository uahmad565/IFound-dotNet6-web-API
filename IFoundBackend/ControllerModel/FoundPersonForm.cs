using IFoundBackend.Model.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;

namespace IFoundBackend.ControllerModel
{
    public class FoundPersonForm
    {
        public IFormFile Image { get; set; }

        public string Base64Image { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }
        public int UserId { get; set; }
        public GenderType Gender { get; set; }
        public string Name { get; set; }
        public TargetType TargetType { get; set; }
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
