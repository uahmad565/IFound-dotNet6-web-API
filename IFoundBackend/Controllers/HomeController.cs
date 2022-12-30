using IFoundBackend.localDatabase;
using IFoundBackend.MxFace;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;
using Microsoft.Extensions.Logging;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IFoundBackend.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public List<FaceIdentity> IdentityList { set; get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            //IDK whether it is correct or not
            
        }

        [HttpGet("/products2/{id}", Name = "Products_List")]
        public IActionResult GetProduct(int id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        //    formData.append("name", credentials.name);
        //formData.append("age", credentials.age);
        //formData.append("city", credentials.city);
        //formData.append("details", credentials.detail);
        //formData.append("postType", credentials.postType);

        [HttpPost("/api/idk")]
        public async Task<string> SearchFace([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {
            //conversion to base64
            string encoded = "";
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    encoded = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                }
            }

            //searching
            string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);
            Task<HttpResponseMessage> response = mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { 1907}, encoded, 1);

            string apiResponse = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(":::::::::::::::Search Face Identity - START:::::::::::::::::::::::::::::\n");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                

                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                foreach (var searchedIdentities in searchFaceIdentityResponse.SearchedIdentities)
                {
                    foreach (var item in searchedIdentities.identityConfidences)
                    {
                        FaceIdentity localIdentity = IdentityList.Find(i => i.IdentityId == item.identity.IdentityId);
                        return localIdentity.Image;
                        //Console.WriteLine("faceConfidence:" + item.confidence);
                        //Console.WriteLine("faceIdentity.FaceIdentityId : {0}, faceIdentity.ExternalId : {1}, faceIdentity.CreatedDate : {2}, " +
                        //    "faceIdentity.UpdatedDate : {3}, faceIdentity.GroupId : {4} ", item.identity.IdentityId, item.identity.ExternalId, item.identity.CreatedDate
                        //    , item.identity.UpdatedDate, item.identity.GroupIds);
                    }
                }

            }
            else
            {
                string externalId = "";
                //Enter post to Group "Lost_Side"
                Task<HttpResponseMessage> response2 =  mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1907 }, externalId, encoded);
                Console.WriteLine("Error message : {0}, StatusCode : {1}", response2.Result.Content, response2.Result.StatusCode);
            }

            return encoded;

        }

        [HttpPost("/searchLost")]
        public async Task<string> FindFoundGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {

            string json = System.IO.File.ReadAllText(@"C:\Users\kabirus\source\repos\IFoundBackend\IFoundBackend\localDatabase\data.json");
            List<FaceIdentity> playerList = JsonConvert.DeserializeObject<List<FaceIdentity>>(json);
            IdentityList = playerList;
            string encoded = "";
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    encoded = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                }
            }

            string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);
            HttpResponseMessage response = await mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> {1907 }, encoded, 1);

            string apiResponse = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(":::::::::::::::Search Face Identity - START:::::::::::::::::::::::::::::\n");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                foreach (var searchedIdentities in searchFaceIdentityResponse.SearchedIdentities)
                {
                    foreach (var item in searchedIdentities.identityConfidences)
                    {
                        if(item.confidence>85)
                        {
                            FaceIdentity localIdentity = IdentityList.Find(i => i.IdentityId == item.identity.IdentityId);
                            return localIdentity.Image;
                        }
                        else
                        {
                            string externalId = "TestDirector";
                            //Enter post to Group "Found"
                            HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1907 }, externalId, encoded);
                            string apiResponse2 = await response2.Content.ReadAsStringAsync();
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);

                                IdentityList.Add(new FaceIdentity
                                {
                                    IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                                    Image = encoded

                                });

                                //Store JSON
                                string resultJson=JsonConvert.SerializeObject(IdentityList);
                                System.IO.File.WriteAllText(@"C:\Users\kabirus\source\repos\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);

                                return "Created";
                            }
                            else
                            {
                                return "Couldn't Register Image";
                                //Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                            }
                        }
                        
                    }
                }
                return "asdas";
            }
            else
            {
                //Failure in Searching
                return "Searching Failed";
            }
        }
    }
    
}
