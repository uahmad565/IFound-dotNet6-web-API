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

        //LostGroup: Id= 1924
        [HttpPost("/searchFound")]
        public async Task<IActionResult> FindLostGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {
            string json = System.IO.File.ReadAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json");
            List<FaceIdentity> IdentityList = JsonConvert.DeserializeObject<List<FaceIdentity>>(json);
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
            HttpResponseMessage response = await mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { 1924 }, encoded, 1);

            string apiResponse = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(":::::::::::::::Search Face Identity - START:::::::::::::::::::::::::::::\n");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);

                if (searchFaceIdentityResponse.SearchedIdentities.Count == 0)
                {
                    string externalId = name + age;
                    //Enter post to Group "Found"
                    HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1907 }, externalId, encoded);

                    string apiResponse2 = await response2.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);

                        IdentityList.Add(new FaceIdentity
                        {
                            IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                            Image = (FormFile)file

                        });

                        //Store JSON
                        string resultJson = JsonConvert.SerializeObject(IdentityList);
                        System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);

                        return Ok(new
                        {
                            message="",
                            faceIdentityInfoResponse= faceIdentityInfoResponse
                        });
                    }
                    else
                    {
                        return BadRequest(); //May be the image quality is bad
                    }
                }

                foreach (var searchedIdentities in searchFaceIdentityResponse.SearchedIdentities)
                {
                    foreach (var item in searchedIdentities.identityConfidences)
                    {
                        if (item.confidence > 70)
                        {
                            FaceIdentity localIdentity = IdentityList.Find(i => i.IdentityId == item.identity.IdentityId);
                            return Ok(new { 
                                message="Found",
                                image=localIdentity.Image
                            });//Local Identity Created
                        }
                        else
                        {
                            string externalId = name + age;
                            //Enter post to Group "Found"
                            HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1907 }, externalId, encoded);

                            string apiResponse2 = await response2.Content.ReadAsStringAsync();
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);

                                IdentityList.Add(new FaceIdentity
                                {
                                    IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                                    Image = (FormFile)file

                                });

                                //Store JSON
                                string resultJson = JsonConvert.SerializeObject(IdentityList);
                                System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);
                                
                                return Ok(new
                                {
                                    message = "",
                                    faceIdentityInfoResponse = faceIdentityInfoResponse
                                });
                            }
                            else
                            {
                                return BadRequest(); //May be the image quality is bad
                            }
                        }

                    }
                }
                return BadRequest();
            }
            else
            {
                return BadRequest();//Failure in Searching
            }

        }

        //FoundGrouID=1907
        [HttpPost("/searchLost")]
        public async Task<IActionResult> FindFoundGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {

            string json = System.IO.File.ReadAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json");
            List<FaceIdentity> IdentityList = JsonConvert.DeserializeObject<List<FaceIdentity>>(json);
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
                
                if(searchFaceIdentityResponse.SearchedIdentities.Count==0)
                {
                    string externalId = name + age;
                    //Enter post to Group "Found"
                    HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1924 }, externalId, encoded);

                    string apiResponse2 = await response2.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);

                        IdentityList.Add(new FaceIdentity
                        {
                            IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                            Image = (FormFile)file

                        });

                        //Store JSON
                        string resultJson = JsonConvert.SerializeObject(IdentityList);
                        System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);

                        return Ok(new { 
                            message="",
                            faceIdentityInfoResponse= faceIdentityInfoResponse
                        });
                    }
                    else
                    {
                        return BadRequest(); //May be the image quality is bad
                    }
                }

                foreach (var searchedIdentities in searchFaceIdentityResponse.SearchedIdentities)
                {
                    foreach (var item in searchedIdentities.identityConfidences)
                    {
                        if(item.confidence>70)
                        {
                            FaceIdentity localIdentity = IdentityList.Find(i => i.IdentityId == item.identity.IdentityId);
                            
                            
                            return Ok(new
                            {
                                message = "Found",
                                image = localIdentity.Image
                            });//Local Identity Created
                        }
                        else
                        {
                            string externalId = name+age;
                            //Enter post to Group "Found"
                            HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1924 }, externalId, encoded);
                            
                            string apiResponse2 = await response2.Content.ReadAsStringAsync();
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);

                                IdentityList.Add(new FaceIdentity
                                {
                                    IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                                    Image = (FormFile)file

                                });

                                //Store JSON
                                string resultJson=JsonConvert.SerializeObject(IdentityList);
                                System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);

                                return Ok(new
                                {
                                    message="",
                                    faceIdentityInfoResponse= faceIdentityInfoResponse
                                });
                            }
                            else
                            {
                                return BadRequest(); //May be the image quality is bad
                            }
                        }
                        
                    }
                }
                return BadRequest();
            }
            else
            {
                return BadRequest();//Failure in Searching
            }
        }
    }
    
}