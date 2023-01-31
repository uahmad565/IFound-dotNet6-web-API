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
using IFoundBackend.MxFace.Utilities;
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
        public readonly int _lostGroupId = 1924;
        public readonly int _foundGroup = 1907;
        public readonly int _confidenceThresh= 70;

        private MXFaceIdentityAPI _mxFaceIdentityAPI;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", _subscriptionKey);
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

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _foundGroup }, encoded, postID.ToString(), _confidenceThresh);

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
                        int postID = await x.createPost(dbContext,data);

                        HttpResponseMessage response = await _mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { _lostGroupId }, encoded, postID.ToString(), _confidenceThresh);

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

        //, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType
        //Search: LostGroup: Id= 1924
        //Register: FoundGroup: Id=1907
        [HttpGet("getCurrentLostPosts")]
        public  IActionResult getCurrentLostPosts()
        {
            var postManager=new PostPersonManager();
            List<PostDto> list=postManager.GetCurrentLostPersonPosts();
            return Ok(list);
        }



        //, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType
        //Search: LostGroup: Id= 1924
        //Register: FoundGroup: Id=1907
        [HttpGet("searchLostPerson")]
        public async Task<IActionResult> searchLostPerson([FromForm] IFormFile data)
        {
            string encoded = "";
            using (var memoryStream = new MemoryStream())
            {
                await data.CopyToAsync(memoryStream);
                byte[] imgdata = memoryStream.ToArray();
                encoded = Convert.ToBase64String(imgdata);
            }
            int limit=3;
            //---
            
            HttpResponseMessage response = await _mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { _foundGroup }, encoded, limit);
            Task<string> apiResponseTask = response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await apiResponseTask;
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                var searchPosts = new Dictionary<string, double>();
                foreach (var lookUpIdentity in searchFaceIdentityResponse.SearchedIdentities)
                {
                    lookUpIdentity.identityConfidences.ForEach(identityConfidence =>
                    {
                        searchPosts.Add(identityConfidence.identity.ExternalId, identityConfidence.confidence);
                    });
                }
                var x = new PostPersonManager();
                var keys = searchPosts.Keys.ToArray();
                
                List<PostDto> MatchedPosts=x.getPosts(keys);
                return Ok(MatchedPosts);
            }
            else
            {
                return BadRequest("Error In Searching.");
            }

        }

        //Search: FoundGroup: Id=1907
        //Register: LostGroup: Id= 1924
        //[HttpPost]
        //public async Task<IActionResult> FindFoundGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        //{

        //    List<FaceIdentity> allIdentities = getLocalIdentities(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json");

        //    string encoded = convertToBase64(file);

        //    string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
        //    MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);

        //    HttpResponseMessage response = await mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { 1907 }, encoded, 1);

        //    string apiResponse = await response.Content.ReadAsStringAsync();

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
        //        FaceIdentity result = searchFace(allIdentities, searchFaceIdentityResponse);
        //        if (result == null) //Search Failed
        //        {
        //            FaceIdentityInfo faceIdentityInfoResponse = await createIdentity(name + age, encoded, mxFaceIdentityAPI, new List<int> { 1924 });
        //            allIdentities.Add(new FaceIdentity
        //            {
        //                IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
        //                Image = encoded

        //            });

        //            //Store JSON
        //            string resultJson = JsonConvert.SerializeObject(allIdentities);
        //            System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);
        //            return Ok(new
        //            {
        //                message = "",
        //                FaceIdentityInfo = faceIdentityInfoResponse
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                message = "Found",
        //                image = result.Image
        //            });
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();//Failure in Searching
        //    }
        //    return BadRequest();
        //}
  
    }
    
}

//Helping Functions

//byte[] imgdata = SqlAccess.GetPhoto("C:\\Users\\kabirus\\Desktop\\UsmanUni\\UsmanFaceIdentity\\usman2.jpg");
//string encoded = Convert.ToBase64String(imgdata);