using IFoundBackend.SqlModels;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using System;

namespace IFoundBackend.Areas.ToDTOs
{
    public static class ConvertToDTOs
    {
        public static SearchedPostDto ToSearchedPostDto(IdentityConfidences identityConfidence, MxFaceIdentity Data)
        {
            
            var imageDto = new ImageDto();
            imageDto.ImageId = (int)Data.Post.ImageId;
            imageDto.Base64String = Convert.ToBase64String(Data.Post.Image.Pic);

            imageDto.ImageId = (int)Data.Post.ImageId;
            imageDto.Base64String = Convert.ToBase64String(Data.Post.Image.Pic);


            var targetPersonDto = new TargetPersonDto();
            targetPersonDto.PersonId = Data.Post.Person.PersonId;
            targetPersonDto.Age = (int)Data.Post.Person.Age;
            targetPersonDto.Gender = Data.Post.Person.Gender;
            targetPersonDto.Relation = Data.Post.Person.Relation;
            targetPersonDto.Description = Data.Post.Person.Description;
            targetPersonDto.Location = Data.Post.Person.Location;
            targetPersonDto.TargetId = Data.Post.Person.TargetId;
            targetPersonDto.Name = Data.Post.Person.Name;

            var searchedPostDto = new SearchedPostDto
            {
                Confidence= identityConfidence.confidence,
                MxIdentityFaceID = Data.FaceIdentityId,
                PostPersonId = Data.PostId,
                ImageDto = imageDto,
                PostDate = Data.Post.PostDate,
                StatusId = (int)Data.Post.StatusId,
                UserID = Data.Post.UserId,
                Phone=Data.Post.Phone,
                TargetPersonDto = targetPersonDto
            };
            return searchedPostDto;
        }
        public static PostDto toDto(MxFaceIdentity Data)
        {
            var imageDto = new ImageDto();
            imageDto.ImageId = (int)Data.Post.ImageId;
            imageDto.Base64String = Convert.ToBase64String(Data.Post.Image.Pic);

            var targetPersonDto = new TargetPersonDto();
            targetPersonDto.PersonId = Data.Post.Person.PersonId;
            targetPersonDto.Age = (int)Data.Post.Person.Age;
            targetPersonDto.Gender = Data.Post.Person.Gender;
            targetPersonDto.Relation = Data.Post.Person.Relation;
            targetPersonDto.Description = Data.Post.Person.Description;
            targetPersonDto.Location = Data.Post.Person.Location;
            targetPersonDto.TargetId = Data.Post.Person.TargetId;
            targetPersonDto.Name = Data.Post.Person.Name;

            var postDto = new PostDto
            {
                MxIdentityFaceID = Data.FaceIdentityId,
                PostPersonId = Data.PostId,
                ImageDto = imageDto,
                PostDate = Data.Post.PostDate,
                StatusId = (int)Data.Post.StatusId,
                UserID = Data.Post.UserId,
                TargetPersonDto = targetPersonDto,
                Phone = Data.Post.Phone
            };

            return postDto;
        }

        public static PostDto ToDto(PostPerson Data)
        {
            var imageDto = new ImageDto();
            imageDto.ImageId = (int)Data.ImageId;
            imageDto.Base64String = Convert.ToBase64String(Data.Image.Pic);

            var targetPersonDto = new TargetPersonDto();
            targetPersonDto.PersonId = Data.Person.PersonId;
            targetPersonDto.Age = (int)Data.Person.Age;
            targetPersonDto.Gender = Data.Person.Gender;
            targetPersonDto.Relation = Data.Person.Relation;
            targetPersonDto.Description = Data.Person.Description;
            targetPersonDto.Location = Data.Person.Location;
            targetPersonDto.TargetId = Data.Person.TargetId;
            targetPersonDto.Name = Data.Person.Name;

            var postDto = new PostDto
            {
                PostPersonId=Data.PostPersonId,
                ImageDto = imageDto,
                PostDate = Data.PostDate,
                StatusId = (int)Data.StatusId,
                UserID = Data.UserId,
                TargetPersonDto = targetPersonDto
            };

            return postDto;
        }
    }
}
