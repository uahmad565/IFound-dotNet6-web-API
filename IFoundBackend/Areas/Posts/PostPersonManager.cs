using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.ControllerModel;
using IFoundBackend.Data;
using IFoundBackend.Model.Enums;
using IFoundBackend.SqlModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConvertToDTOs = IFoundBackend.Areas.ToDTOs.ConvertToDTOs;
namespace IFoundBackend.Areas.Posts
{
    public class PostPersonManager
    {
        public PostPersonManager()
        {
        }
        public List<PostDto> getPosts(string[] ids)
        {
            var result = new List<PostDto>();

            //Querying with LINQ to Entities 
            using (var context = new IFoundContext())
            {
                foreach (var idInString in ids)
                {
                    int id = Convert.ToInt32(idInString);
                    var Data = (from request in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                where request.PostId == id
                                select request).FirstOrDefault();

                    PostDto postDto = ConvertToDTOs.toDto(Data);
                    result.Add(postDto);
                }
            }
            return result;
        }

        public List<PostDto> GetCurrentPersonPosts(TargetType targetType)
        {
            using (var context = new IFoundContext())
            {
                var mxFaceIdentities = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                        where x.Post.Person.TargetId == (int)(targetType)
                                        select x).ToList();

                List<PostDto> lostPosts = new List<PostDto>();
                foreach (var item in mxFaceIdentities)
                {
                    PostDto result = ConvertToDTOs.toDto(item);
                    lostPosts.Add(result);
                }
                return lostPosts;
            }
        }

        public List<PostDto> GetUserActiveCases(TargetType targetType, int userID)
        {
            using (var context = new IFoundContext())
            {
                var mxFaceIdentities = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(mxFaceIdentity => mxFaceIdentity.Post.Person.Target)
                .Include(d => d.Post.Status)
                                        where (x.Post.UserId == userID && x.Post.Person.TargetId == (int)targetType)
                                        select x).ToList();

                List<PostDto> lostPosts = new List<PostDto>();
                foreach (var item in mxFaceIdentities)
                {
                    PostDto result = ConvertToDTOs.toDto(item);
                    lostPosts.Add(result);
                }
                return lostPosts;
            }

        }
        public List<SearchedPostDto> GetSearchedPosts(List<IdentityConfidences> identityConfidences)
        {
            List<SearchedPostDto> result=new List<SearchedPostDto>(identityConfidences.Count);
            using (var context = new IFoundContext())
            {
                
                foreach (var identityConfidence in identityConfidences)
                {
                    int id = Convert.ToInt32(identityConfidence.identity.ExternalId);
                    
                    var Data = (from request in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                where request.PostId == id
                                select request).FirstOrDefault();

                    result.Add(ConvertToDTOs.ToSearchedPostDto(identityConfidence, Data));
                }
                
            }
            return result;
        }

        public async Task<int> createPost(IFoundContext dbContext, PersonForm data)
        {

            var postPerson = new PostPerson();
            var image = new SqlModels.Image();

            var imageTask = data.convertToImage(data.Image);

            postPerson.StatusId = (int)data.PostStatus;
            postPerson.PostDate = DateTime.Now;
            postPerson.UserId = (int)data.UserId;

            var targetPerson = new TargetPerson();
            targetPerson.Name = data.Name;
            targetPerson.Gender = data.Gender.ToString();
            targetPerson.Age = data.Age;
            targetPerson.Relation = data.Relation.ToString();
            targetPerson.Description = data.Description;
            targetPerson.Location = data.Location;
            targetPerson.TargetId = (int)data.TargetType;
            postPerson.Person = targetPerson;
            byte[] imageData = await imageTask;
            if (imageData != null)
            {
                image.Pic = imageData;
            }
            //image.Pic = System.Convert.FromBase64String(data.Base64Image);
            postPerson.Image = image;


            dbContext.PostPeople.Add(postPerson);
            dbContext.SaveChanges();
            return postPerson.PostPersonId;

        }

        public void createFoundPost()
        {

        }
        public void updatePost(int id)
        {

        }
        public void deletePostById(int id)
        {

        }
        public void getPostById(int id)
        {

        }

        public void getAllLostPosts(int limit)
        {

        }

    }
}
