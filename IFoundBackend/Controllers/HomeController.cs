using IFoundBackend.Areas.Posts;
using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.DTOs;
using IFoundBackend.Model.Enums;
using IFoundBackend.MxFace;
using IFoundBackend.SqlModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using IFoundBackend.Auth;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IFoundBackend.Areas.Help;
using IFoundBackend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace IFoundBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public readonly int _lostGroupId = 22;
        public readonly int _foundGroup = 23;
        public readonly int _confidenceThresh = 100;
        private readonly UserManager<ApplicationUser> _userManager;

        private IPostManager _postManager { get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _lostGroupId = int.Parse(configuration["MXFaceConfiguration:lostGroupId"]);
            _foundGroup = int.Parse(configuration["MXFaceConfiguration:foundGroupId"]);
            _confidenceThresh = int.Parse(configuration["MXFaceConfiguration:confidenceThresh"]);
            _userManager = userManager;
            string _subscriptionKey = configuration["MXFaceConfiguration:subscriptionKey"];
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI(configuration["MXFaceConfiguration:URL"], _subscriptionKey);
            _postManager = new PostPersonManager(mxFaceIdentityAPI, _userManager);
        }

        //public HomeController()
        //{
        //    string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
        //    MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v3/", _subscriptionKey);
        //    _mxFaceIdentityAPI = mxFaceIdentityAPI;
        //}

        [AllowAnonymous]
        [HttpGet]
        public string Index()
        {
            return "<h1>Server is running </h1>";
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("createFoundPersonForm")]
        public async Task<IActionResult> CreateFoundPersonIdentity([FromForm] PersonForm data)
        {
            try
            {
                string tokenUserId = this.GetTokenUserId();
                var response = await _postManager.CreatePost(data, _foundGroup, tokenUserId);
                if (response.IsSuccessStatusCode)
                {
                    return Ok(response);
                }
                return StatusCode(((int)response.StatusCode), response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("createLostPersonForm")]
        public async Task<IActionResult> CreateLostPersonIdentity([FromForm] PersonForm data)
        {
            try
            {
                string tokenUserId = this.GetTokenUserId();
                var response = await _postManager.CreatePost(data, _lostGroupId, tokenUserId);
                if (response.IsSuccessStatusCode)
                {
                    return Ok(response);
                }
                return StatusCode(((int)response.StatusCode), response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getCurrentFoundPosts")]
        public IActionResult GetCurrentFoundPosts()
        {
            List<PostDto> list = _postManager.GetCurrentPersonPosts(TargetType.FOUND);

            // Configure the JSON serializer to use UTC format for DateTime properties
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return new JsonResult(list, settings);
        }

        [HttpGet("filterPosts")]
        public IActionResult ApplyFilterPosts(string City, string Name, int MinAge, int MaxAge, GenderType Gender, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize = 10)
        {

            List<PostDto> list = _postManager.GetCurrentPersonPostsFilters(TargetType.LOST, City, Name, MinAge, MaxAge, Gender, FromDate, ToDate, PageNo, PageSize);

            // Configure the JSON serializer to use UTC format for DateTime properties
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return new JsonResult(list, settings);
        }


        [Authorize]
        [HttpGet("getCurrentLostPosts")]
        public IActionResult GetCurrentLostPosts()
        {
            List<PostDto> list = _postManager.GetCurrentPersonPosts(TargetType.LOST);

            // Configure the JSON serializer to use UTC format for DateTime properties
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return new JsonResult(list, settings);
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet("activeCases/{postStatus}/{targetType}")]
        public IActionResult GetUserActiveCases([FromRoute] Model.Enums.PostStatus postStatus, [FromRoute] TargetType targetType)
        {
            string tokenUserId = this.GetTokenUserId();
            List<PostDto> list = _postManager.GetUserActiveCases(postStatus, targetType, tokenUserId);
            return Ok(list);
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("searchPerson/{targetType}")]
        public async Task<IActionResult> SearchLostPerson([FromForm] string encoded, [FromRoute] TargetType targetType)
        {

            int findGroupID = 0;
            if (targetType == TargetType.FOUND)
                findGroupID = _lostGroupId;
            else if (targetType == TargetType.LOST)
                findGroupID = _foundGroup;

            int limit = 2;
            bool returnConfidence = true;
            var result = await _postManager.SearchPosts(findGroupID, encoded, limit, returnConfidence);
            if (result != null)
                return Ok(result);
            return BadRequest("Error In Searching Make sure to send clear Image");
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPut("UpdatePostStatus/{postId}")]
        public ActionResult UpdatePostStatus(int postId, [FromBody] Model.Enums.PostStatus postStatus)
        {
            try
            {
                _postManager.UpdatePostStatus(postId, postStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [Authorize]
        [HttpDelete("DeleteCurrentPost/{postId}")]
        public async Task<ActionResult> DeletePost([FromRoute] int postId)
        {
            try
            {
                var response = await _postManager.DeletePost(postId);
                if (response.IsSuccessStatusCode)
                    return Ok(response);
                return StatusCode(((int)response.StatusCode), response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = UserRoles.User)]
        // GET: api/PostPersons/5
        [HttpGet("GetCurrentPostPerson/{postId}/{postType}")]
        public async Task<ActionResult<PostPerson>> GetCurrentPostPerson(int postId, TargetType postType)
        {
            try
            {
                var result = await _postManager.GetCurrentPostById(postId, postType);

                if (result == null)
                    return NotFound();

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

        [Authorize(Roles = UserRoles.User)]
        [HttpGet("GetPostPerson")]
        public async Task<ActionResult<PostPerson>> GetPostPerson(int id)
        {
            try
            {
                var result = await _postManager.GetPostById(id);
                if (result == null)
                    return NotFound();

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

        [Authorize]
        [HttpGet("DashboardStats")]
        public IActionResult GetDashboardStats()
        {
            try
            {
                string tokenUserId = GetTokenUserId();
                StatDto dto = new()
                {
                    UserActiveLostCasesCount = _postManager.GetUserActiveCasesCount(TargetType.LOST, tokenUserId),
                    UserActiveFoundCasesCount = _postManager.GetUserActiveCasesCount(TargetType.FOUND, tokenUserId),
                    UserUnresolvedCasesCount = _postManager.GetUserUnresolvedCasesCount(tokenUserId),
                    UserResolvedCasesCount = _postManager.GetUserResolvedCasesCount(tokenUserId),
                    AllActiveLostPostCount = _postManager.GetActivePostCount(TargetType.LOST),
                    AllActiveFoundPostCount = _postManager.GetActivePostCount(TargetType.FOUND),
                    AllResolvedPostCount = _postManager.GetAllResolvedCasesCount(),
                    AllUnResolvedPostCount = _postManager.GetAllUnResolvedCasesCount()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        #endregion

        #region Private Methods
        private string GetTokenUserId()
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return JWTHandler.GetUserID(token);
        }
        #endregion
    }

}
