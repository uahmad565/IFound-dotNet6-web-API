using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.DTOs;
using IFoundBackend.Model.Enums;
using IFoundBackend.MxFace;
using IFoundBackend.SqlModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using ConvertToDTOs = IFoundBackend.Areas.ToDTOs.ConvertToDTOs;
using IFoundBackend.Model.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace IFoundBackend.Areas.Posts
{
    public class PostPersonManager : IPostManager
    {
        private static MXFaceIdentityAPI _mxFaceIdentityAPI = null;

        public PostPersonManager(MXFaceIdentityAPI mxFaceIdentityAPI)
        {
            _mxFaceIdentityAPI = mxFaceIdentityAPI;
        }

        public async Task<List<SearchedPostDto>> SearchPosts(int groupID,string encoded,int limit,bool returnConfidence)
        {
            HttpResponseMessage response = await _mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { groupID }, encoded, limit, returnConfidence);
            Task<string> apiResponseTask = response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await apiResponseTask;
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                List<SearchedPostDto> searchedPostDtos = new List<SearchedPostDto>();
                foreach (var lookUpIdentity in searchFaceIdentityResponse.SearchedIdentities)
                {
                    searchedPostDtos.AddRange(GetSearchedPosts(lookUpIdentity.identityConfidences));
                }

                return searchedPostDtos;
            }
            else
            {
                return  null;
            }
        }

        public async Task<HttpResponseMessage> CreatePost(PersonForm data, int groupID, string tokenUserId)
        {
            string encoded = data.convertToBase64(data.Image);
            //string encoded = data.Base64Image;
            using (var dbContext = new IfoundContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        int postID = await CreatePost(dbContext, data,tokenUserId);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { groupID }, encoded, postID.ToString(), true);

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {

                            var mxFaceIdentity = new MxFaceIdentity
                            {
                                FaceIdentityId = (int)faceIdentityInfoResponse.FaceIdentityId,
                                PostId = postID
                            };
                            dbContext.MxFaceIdentities.Add(mxFaceIdentity);
                            dbContext.SaveChanges();
                            transaction.Commit();
                            return response;
                        }
                        else
                        {
                            transaction.Rollback();
                            return new HttpResponseMessage
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                ReasonPhrase = faceIdentityInfoResponse.ErrorMessage
                            };
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }
        }

        public async Task<HttpResponseMessage> DeletePost(int postId)
        {
            using (var dbContext = new IfoundContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        int identityId = DeletePost(dbContext, postId);
                        if (identityId == -1)
                            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };

                        HttpResponseMessage response = await _mxFaceIdentityAPI.DeleteFaceIdentity(identityId);
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            FaceIdentityInfo deleteFaceIdentityInfo = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
                            if (string.IsNullOrEmpty(deleteFaceIdentityInfo.ErrorMessage))
                            {
                                //Console.WriteLine("face.Identity : {0} deleted successfully.", faceIdentityId);
                                transaction.Commit();
                                return new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent};
                            }
                            else
                            {
                                //logging("Error message : {0}", deleteFaceIdentityInfo.ErrorMessage);
                                transaction.Rollback();
                                return new HttpResponseMessage { StatusCode=HttpStatusCode.NotFound,ReasonPhrase= deleteFaceIdentityInfo.ErrorMessage };
                            }

                        }
                        else
                        {
                            //logging("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound};
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
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
                                        where x.Post.Person.TargetId == (int)(targetType) && x.Post.StatusId == (int)Model.Enums.PostStatus.Unresolved
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
        public int GetUserActiveCasesCount(TargetType targetType, string tokenUserId)
        {
            using (var context = new IfoundContext())
            {
                return (from x in context.MxFaceIdentities.Include(c => c.Post.Person)
                        where (x.Post.UserId.Equals(tokenUserId) && x.Post.Person.TargetId == (int)targetType)
                        select x).Count();
            }

        }
        public int GetUserUnresolvedCasesCount(string userID)
        {
            using (var context = new IfoundContext())
            {
                return (from x in context.MxFaceIdentities.Include(c => c.Post.Person)
                        where (x.Post.UserId.Equals(userID) && (x.Post.Person.TargetId == (int)TargetType.LOST || x.Post.Person.TargetId == (int)TargetType.FOUND))
                        select x).Count();
            }

        }
        public int GetUserResolvedCasesCount(string userID)
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
                        where (x.Post.Person.TargetId == (int)targetType)
                        select x).Count();
            }
        }
        #endregion

        public List<PostDto> GetUserActiveCases(Model.Enums.PostStatus postStatus, TargetType targetType, string tokenUserID)
        {
            using (var context = new IfoundContext())
            {
                var mxFaceIdentities = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(mxFaceIdentity => mxFaceIdentity.Post.Person.Target)
                                            .Include(d => d.Post.Status)
                                        where (x.Post.UserId.Equals(tokenUserID) && x.Post.Status.StatusId == (int)postStatus && x.Post.Person.TargetId == (int)targetType)
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
            using (var context = new IfoundContext())
            {
                context.PostPeople.Where(x => x.PostPersonId == postId).ExecuteUpdate(x => x.SetProperty(y => y.StatusId, (int)postStatus));
            }
        }

        #region Private helping methods
        private int DeletePost(IfoundContext context, int postId)
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

        private async Task<int> CreatePost(IfoundContext dbContext, PersonForm data, string tokenUserId)
        {

            var postPerson = new PostPerson();
            var image = new SqlModels.Image();

            var imageTask = data.convertToImage(data.Image);

            postPerson.StatusId = (int)data.PostStatus;
            postPerson.PostDate = DateTime.Now;
            postPerson.UserId = tokenUserId;

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
            postPerson.Phone = data.Phone;

            dbContext.PostPeople.Add(postPerson);
            dbContext.SaveChanges();
            return postPerson.PostPersonId;

        }
        #endregion

        private List<SearchedPostDto> GetSearchedPosts(List<IdentityConfidences> identityConfidences)
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

        public PostDto GetCurrentPostById(int id, TargetType targetType)
        {
            using (var context = new IfoundContext())
            {
                var mxFaceIdentity = (from x in context.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status)
                                      where x.PostId == id && x.Post.Person.TargetId == (int)targetType
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
