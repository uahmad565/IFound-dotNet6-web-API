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
            using (var context = new IfoundContext())
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
            using (var context = new IfoundContext())
            {
                var mxFaceIdentities = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                        where x.Post.Person.TargetId == (int)(targetType) && x.Post.StatusId==(int)Model.Enums.PostStatus.Unresolved 
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

        #region Statistics
        public int GetUserActiveCasesCount(TargetType targetType, int userID)
        {
            using (var context = new IfoundContext())
            {
                return (from x in context.MxFaceIdentities.Include(c => c.Post.Person)
                                        where (x.Post.UserId == userID && x.Post.Person.TargetId == (int)targetType)
                                        select x).Count();
            }

        }
        public int GetUserUnresolvedCasesCount(int userID)
        {
            using (var context = new IfoundContext())
            {
                return (from x in context.MxFaceIdentities.Include(c => c.Post.Person)
                        where (x.Post.UserId == userID && (x.Post.Person.TargetId == (int)TargetType.LOST || x.Post.Person.TargetId == (int)TargetType.FOUND))
                        select x).Count();
            }

        }
        public int GetUserResolvedCasesCount(int userID)
        {
            return 0;

        }

        public int GetAllResolvedCasesCount()
        {
            return 0;

        }

        public int GetAllUnResolvedCasesCount()
        {
            return 0;

        }

        public int GetActivePostCount(TargetType targetType)
        {
            using (var context = new IfoundContext())
            {
                return (from x in context.MxFaceIdentities.Include(c => c.Post.Person)
                        where ( x.Post.Person.TargetId == (int)targetType)
                        select x).Count();
            }
        }
        #endregion

        public List<PostDto> GetUserActiveCases(Model.Enums.PostStatus postStatus,TargetType targetType, int userID)
        {
            using (var context = new IfoundContext())
            {
                var mxFaceIdentities = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(mxFaceIdentity => mxFaceIdentity.Post.Person.Target)
                                            .Include(d => d.Post.Status)
                                        where (x.Post.UserId == userID && x.Post.Status.StatusId == (int)postStatus && x.Post.Person.TargetId == (int)targetType)
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

        public void UpdatePostStatus(int postId, Model.Enums.PostStatus postStatus)
        {
            using (var context=new IfoundContext())
            {
                context.PostPeople.Where(x => x.PostPersonId == postId).ExecuteUpdate(x =>x.SetProperty(y=>y.StatusId, (int)postStatus));
            }
        }
        public List<SearchedPostDto> GetSearchedPosts(List<IdentityConfidences> identityConfidences)
        {
            List<SearchedPostDto> result = new List<SearchedPostDto>(identityConfidences.Count);
            using (var context = new IfoundContext())
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

        public int DeletePost(IfoundContext context,int postId)
        {
            var mxFaceIdentity = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                           .Include(c => c.Post.Person)
                                  where x.PostId == postId
                                  select x).FirstOrDefault();
            if (mxFaceIdentity == null)
                return -1;

            context.PostPeople.Remove(mxFaceIdentity.Post);
            context.TargetPeople.Remove(mxFaceIdentity.Post.Person);
            context.Images.Remove(mxFaceIdentity.Post.Image);
            context.SaveChanges();
            return mxFaceIdentity.FaceIdentityId;
        }

        public async Task<int> createPost(IfoundContext dbContext, PersonForm data)
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
            postPerson.Phone= data.Phone;

            dbContext.PostPeople.Add(postPerson);
            dbContext.SaveChanges();
            return postPerson.PostPersonId;

        }

        public PostDto GetCurrentPostById(int id,TargetType targetType)
        {
            using (var context = new IfoundContext())
            {
                var mxFaceIdentity = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                      where x.PostId == id && x.Post.Person.TargetId== (int)targetType
                                      select x).FirstOrDefault();
                if (mxFaceIdentity == null)
                    return null;

                PostDto postDto = ConvertToDTOs.toDto(mxFaceIdentity);
                return postDto;
            }
        }

        public PostDto GetPostById(int id)
        {
            using (var context = new IfoundContext())
            {
                var mxFaceIdentity = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                      where x.PostId == id
                                      select x).FirstOrDefault();
                if (mxFaceIdentity == null)
                    return null;

                PostDto postDto = ConvertToDTOs.toDto(mxFaceIdentity);
                return postDto;
            }
        }

    }
}
