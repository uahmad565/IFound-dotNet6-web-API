using IFoundBackend.Areas.MxFaceManager;
using IFoundBackend.Areas.Posts;
using IFoundBackend.Areas.ToDTOs;
using IFoundBackend.ControllerModel;
using IFoundBackend.Data;
using IFoundBackend.Model;
using IFoundBackend.Model.Abstracts;
using IFoundBackend.Model.Enums;
using IFoundBackend.Model.Posts;
using IFoundBackend.Model.Targets;
using IFoundBackend.MxFace;
using IFoundBackend.SqlModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Docs.Samples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

//Scaffolding Command
//Scaffold-DbContext "Data Source=LHE-LT-UKABIR;Initial Catalog=IFound;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir SqlModels -Force
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

    
        [HttpPost("createFoundPersonForm")]
        //public async Task<IActionResult> createLostPersonIdentity([Bind("Image,Description ,Location, Age, UserId, Gender, Relation, Name, targetType")] LostPersonForm data)
        public async Task<IActionResult> createFoundPersonIdentity([FromForm] PersonForm data)
        {
            data.TargetType = TargetType.FOUND;
            string encoded = data.convertToBase64(data.Image);
            //string encoded = data.Base64Image;
            using (var dbContext = new IFoundContext())
            {

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var x = new PostPersonManager();
                        int postID = await x.createPost(dbContext, data);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _foundGroup }, encoded, postID.ToString(), true);

                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
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
                            return BadRequest(response);

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
        public async Task<IActionResult> createLostPersonIdentity([FromForm] PersonForm data)
        {
            data.TargetType = TargetType.LOST;

            string encoded = data.convertToBase64(data.Image);
            //string encoded = data.Base64Image;
            using (var dbContext = new IFoundContext())
            {

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var x = new PostPersonManager();
                        int postID = await x.createPost(dbContext, data);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _lostGroupId }, encoded, postID.ToString(), true);

                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
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
                            return BadRequest(response);

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
            return Ok(list);
        }

        [HttpGet("activeCases")]
        public IActionResult GetUserActiveCases(TargetType targetType,int id)
        {
            var postManager = new PostPersonManager();
            List<PostDto> list = postManager.GetUserActiveCases(targetType, id);
            return Ok(list);
        }

        
        [HttpPost("searchLostPerson")]
        //public async Task<IActionResult> SearchLostPerson([FromForm] IFormFile data)
        public async Task<IActionResult> SearchLostPerson([FromForm] string encoded, [FromForm] TargetType targetType)
        {
           
            int findGroupID = 0;
            if(targetType== TargetType.FOUND)
                findGroupID = _lostGroupId;
            else if(targetType== TargetType.LOST)
                findGroupID = _foundGroup;

            int limit = 2;
            bool returnConfidence = true;
            HttpResponseMessage response = await _mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { findGroupID }, encoded, limit, returnConfidence);
            Task<string> apiResponseTask = response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await apiResponseTask;
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                List<SearchedPostDto> searchedPostDtos= new List<SearchedPostDto>();
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




    }

}
