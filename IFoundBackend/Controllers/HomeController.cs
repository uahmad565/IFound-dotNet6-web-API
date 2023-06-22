using IFoundBackend.Areas.Posts;
using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.ControllerModel;
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


        private IPostManager _postManager { get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _lostGroupId = int.Parse(configuration["MXFaceConfiguration:lostGroupId"]);
            _foundGroup = int.Parse(configuration["MXFaceConfiguration:foundGroupId"]);
            _confidenceThresh = int.Parse(configuration["MXFaceConfiguration:confidenceThresh"]);
            string _subscriptionKey = configuration["MXFaceConfiguration:subscriptionKey"];
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI(configuration["MXFaceConfiguration:URL"], _subscriptionKey);
            _postManager = new PostPersonManager(mxFaceIdentityAPI);
        }

        //public HomeController()
        //{
        //    string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
        //    MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v3/", _subscriptionKey);
        //    _mxFaceIdentityAPI = mxFaceIdentityAPI;
        //}


        [HttpPost("createFoundPersonForm")]
        public async Task<IActionResult> CreateFoundPersonIdentity([FromForm] PersonForm data)
        {
            try
            {
                var response = await _postManager.CreatePost(data, _foundGroup);
                if(response.IsSuccessStatusCode) { 
                    return Ok(response);
                }
                return StatusCode(((int)response.StatusCode), response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("createLostPersonForm")]
        public async Task<IActionResult> CreateLostPersonIdentity([FromForm] PersonForm data)
        {
            try
            {
                var response = await _postManager.CreatePost(data, _lostGroupId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getCurrentFoundPosts")]
        public IActionResult GetCurrentFoundPosts()
        {
            List<PostDto> list = _postManager.GetCurrentPersonPosts(TargetType.FOUND);
            return Ok(list);
        }


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

            //var json = JsonConvert.SerializeObject(list, settings);

            return new JsonResult(list, settings);

        }

        [HttpGet("activeCases/{postStatus}")]
        public IActionResult GetUserActiveCases([FromRoute] Model.Enums.PostStatus postStatus, [FromQuery] TargetType targetType, [FromQuery] int id)
        {
            List<PostDto> list = _postManager.GetUserActiveCases(postStatus, targetType, id);
            return Ok(list);
        }

        [HttpPost("searchLostPerson")]
        public async Task<IActionResult> SearchLostPerson([FromForm] string encoded, [FromForm] TargetType targetType)
        {

            int findGroupID = 0;
            if (targetType == TargetType.FOUND)
                findGroupID = _lostGroupId;
            else if (targetType == TargetType.LOST)
                findGroupID = _foundGroup;

            int limit = 2;
            bool returnConfidence = true;
            var result=await _postManager.SearchPosts(findGroupID,encoded,limit, returnConfidence);
            if (result != null)
                return Ok(result);
            return BadRequest("Error In Searching Make sure to send clear Image");
        }

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

        [HttpDelete("DeleteCurrentPost")]
        public async Task<ActionResult> DeletePost(int postId)
        {
            try
            {
               var response=await _postManager.DeletePost(postId);
                if (response.IsSuccessStatusCode)
                    return Ok(response);
                return StatusCode(((int)response.StatusCode),response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET: api/PostPersons/5
        [HttpGet("GetCurrentPostPerson")]
        public ActionResult<PostPerson> GetCurrentPostPerson(int postId, TargetType postType)
        {
            try
            {
                var result = _postManager.GetCurrentPostById(postId, postType);

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
                var result = _postManager.GetPostById(id);

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
                StatDto dto = new()
                {
                    UserActiveLostCasesCount = _postManager.GetUserActiveCasesCount(TargetType.LOST, userId),
                    UserActiveFoundCasesCount = _postManager.GetUserActiveCasesCount(TargetType.FOUND, userId),
                    UserUnresolvedCasesCount = _postManager.GetUserUnresolvedCasesCount(userId),
                    UserResolvedCasesCount = _postManager.GetUserResolvedCasesCount(userId),
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
    }

}
