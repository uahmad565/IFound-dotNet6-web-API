using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.DTOs;
using IFoundBackend.Model.Enums;
using IFoundBackend.SqlModels;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace IFoundBackend.Areas.Posts
{
    public interface IPostManager
    {
        public Task<HttpResponseMessage> CreatePost(PersonForm data, int groupID, string tokenUserId);
        public List<PostDto> getPosts(string[] ids);
        public List<PostDto> GetCurrentPersonPosts(TargetType targetType);
        public int GetUserActiveCasesCount(TargetType targetType, string tokenUserId);
        public int GetUserUnresolvedCasesCount(string userID);
        public int GetUserResolvedCasesCount(string userID);
        public int GetAllResolvedCasesCount();
        public int GetAllUnResolvedCasesCount();
        public int GetActivePostCount(TargetType targetType);
        public List<PostDto> GetUserActiveCases(Model.Enums.PostStatus postStatus, TargetType targetType, string tokenUserID);
        public void UpdatePostStatus(int postId, Model.Enums.PostStatus postStatus);
        public Task<List<SearchedPostDto>> SearchPosts(int groupID, string encoded, int limit, bool returnConfidence);

        public Task<HttpResponseMessage> DeletePost(int postId);
        public PostDto GetCurrentPostById(int id, TargetType targetType);
        public PostDto GetPostById(int id);

    }
}
