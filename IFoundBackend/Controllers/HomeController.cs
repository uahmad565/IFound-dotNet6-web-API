using IFoundBackend.Areas.Posts;
using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.ControllerModel;
using IFoundBackend.Model.Enums;
using IFoundBackend.MxFace;
using IFoundBackend.SqlModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;


//Scaffolding Command
//Scaffold-DbContext "Data Source=LHE-LT-UKABIR;Initial Catalog=IFound;Integrated Security=True;Encrypt=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir SqlModels -Force
namespace IFoundBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public readonly int _lostGroupId = 22;
        public readonly int _foundGroup = 23;
        public readonly int _confidenceThresh = 100;

        private MXFaceIdentityAPI _mxFaceIdentityAPI;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v3/", _subscriptionKey);
            _mxFaceIdentityAPI = mxFaceIdentityAPI;
        }

        //public HomeController()
        //{
        //    string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
        //    MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v3/", _subscriptionKey);
        //    _mxFaceIdentityAPI = mxFaceIdentityAPI;
        //}


        [HttpPost("createFoundPersonForm")]
        //public async Task<IActionResult> createLostPersonIdentity([Bind("Image,Description ,Location, Age, UserId, Gender, Relation, Name, targetType")] LostPersonForm data)
        public async Task<IActionResult> createFoundPersonIdentity([FromForm] PersonForm data)
        {
            string encoded = data.convertToBase64(data.Image);
            //string encoded = data.Base64Image;
            using (var dbContext = new IfoundContext())
            {

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var x = new PostPersonManager();
                        int postID = await x.createPost(dbContext, data);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _foundGroup }, encoded, postID.ToString(), true);

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
                            return Ok(response);
                        }
                        else
                        {
                            transaction.Rollback();
                            return BadRequest(faceIdentityInfoResponse.ErrorMessage);

                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ex);

                    }
                }

            }
        }


        [HttpPost("createLostPersonForm")]
        //public async Task<IActionResult> createLostPersonIdentity([Bind("Image,Description ,Location, Age, UserId, Gender, Relation, Name, targetType")] LostPersonForm data)
        public async Task<IActionResult> CreateLostPersonIdentity([FromForm] PersonForm data)
        {
            string encoded = data.convertToBase64(data.Image);

            using (var dbContext = new IfoundContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var x = new PostPersonManager();
                        int postID = await x.createPost(dbContext, data);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _lostGroupId }, encoded, postID.ToString(), true);

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
                            return Ok(response);
                        }
                        else
                        {
                            transaction.Rollback();
                            return BadRequest(faceIdentityInfoResponse.ErrorMessage);

                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ex.Message);

                    }
                }

            }
        }

        [HttpGet("getCurrentFoundPosts")]
        public IActionResult GetCurrentFoundPosts()
        {
            var postManager = new PostPersonManager();
            List<PostDto> list = postManager.GetCurrentPersonPosts(TargetType.FOUND);
            return Ok(list);
        }


        [HttpGet("getCurrentLostPosts")]
        public IActionResult GetCurrentLostPosts()
        {
            var postManager = new PostPersonManager();
            List<PostDto> list = postManager.GetCurrentPersonPosts(TargetType.LOST);

            // Configure the JSON serializer to use UTC format for DateTime properties
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            //var json = JsonConvert.SerializeObject(list, settings);

            return new JsonResult(list, settings);

        }

        [HttpGet("activeCases/{postStatus}")]
        public IActionResult GetUserActiveCases([FromRoute] Model.Enums.PostStatus postStatus, [FromQuery] TargetType targetType, [FromQuery] int id)
        {
            var postManager = new PostPersonManager();
            List<PostDto> list = postManager.GetUserActiveCases(postStatus, targetType, id);
            return Ok(list);
        }

        [HttpPost("searchLostPerson")]
        //public async Task<IActionResult> SearchLostPerson([FromForm] IFormFile data)
        public async Task<IActionResult> SearchLostPerson([FromForm] string encoded, [FromForm] TargetType targetType)
        {

            int findGroupID = 0;
            if (targetType == TargetType.FOUND)
                findGroupID = _lostGroupId;
            else if (targetType == TargetType.LOST)
                findGroupID = _foundGroup;

            int limit = 2;
            bool returnConfidence = true;
            HttpResponseMessage response = await _mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { findGroupID }, encoded, limit, returnConfidence);
            Task<string> apiResponseTask = response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await apiResponseTask;
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                List<SearchedPostDto> searchedPostDtos = new List<SearchedPostDto>();
                foreach (var lookUpIdentity in searchFaceIdentityResponse.SearchedIdentities)
                {
                    var x = new PostPersonManager();
                    searchedPostDtos.AddRange(x.GetSearchedPosts(lookUpIdentity.identityConfidences));
                }

                return Ok(searchedPostDtos);
            }
            else
            {
                return BadRequest("Error In Searching.");
            }

        }

        [HttpPut("UpdatePostStatus/{postId}")]
        public ActionResult UpdatePostStatus(int postId, [FromBody] Model.Enums.PostStatus postStatus)
        {
            try
            {
                var postPersonManager = new PostPersonManager();
                postPersonManager.UpdatePostStatus(postId, postStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("DeleteCurrentPost")]
        public async Task<ActionResult> DeletePost(int postId)
        {
            using (var dbContext = new IfoundContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var x = new PostPersonManager();
                        int identityId = x.DeletePost(dbContext, postId);
                        if (identityId == -1)
                            return NotFound();

                        HttpResponseMessage response = await _mxFaceIdentityAPI.DeleteFaceIdentity(identityId);
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            FaceIdentityInfo deleteFaceIdentityInfo = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
                            if (string.IsNullOrEmpty(deleteFaceIdentityInfo.ErrorMessage))
                            {
                                //Console.WriteLine("face.Identity : {0} deleted successfully.", faceIdentityId);
                                transaction.Commit();
                                return NoContent();
                            }
                            else
                            {
                                //logging("Error message : {0}", deleteFaceIdentityInfo.ErrorMessage);
                                transaction.Rollback();
                                return NotFound();
                            }

                        }
                        else
                        {
                            //logging("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                            return NotFound();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, ex.Message);

                    }
                }
            }
        }


        // GET: api/PostPersons/5
        [HttpGet("GetCurrentPostPerson")]
        public ActionResult<PostPerson> GetCurrentPostPerson(int postId, TargetType postType)
        {
            try
            {
                PostPersonManager postManager = new();
                var result = postManager.GetCurrentPostById(postId, postType);

                if (result == null)
                {
                    return NotFound();
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                return new JsonResult(result, settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

        }

        // GET: api/PostPersons/5
        [HttpGet("GetPostPerson")]
        public ActionResult<PostPerson> GetPostPerson(int id)
        {
            try
            {
                PostPersonManager postManager = new();
                var result = postManager.GetPostById(id);

                if (result == null)
                {
                    return NotFound();
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                return new JsonResult(result, settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

        }

        #region Statistics Apis

        [HttpGet("DashboardStats/{userId}")]
        public IActionResult GetDashboardStats(int userId)
        {
            try
            {
                var postManager = new PostPersonManager();
                StatDto dto = new StatDto();
                dto.UserActiveLostCasesCount = postManager.GetUserActiveCasesCount(TargetType.LOST, userId);
                dto.UserActiveFoundCasesCount = postManager.GetUserActiveCasesCount(TargetType.FOUND, userId);
                dto.UserUnresolvedCasesCount = postManager.GetUserUnresolvedCasesCount(userId);
                dto.UserResolvedCasesCount = postManager.GetUserResolvedCasesCount(userId);

                dto.AllActiveLostPostCount = postManager.GetActivePostCount(TargetType.LOST);
                dto.AllActiveFoundPostCount = postManager.GetActivePostCount(TargetType.FOUND);
                dto.AllResolvedPostCount = postManager.GetAllResolvedCasesCount();
                dto.AllUnResolvedPostCount = postManager.GetAllUnResolvedCasesCount();

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        #endregion
    }

}
