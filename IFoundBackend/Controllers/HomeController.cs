using IFoundBackend.localDatabase;
using IFoundBackend.MxFace;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace IFoundBackend.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
      
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            //IDK whether it is correct or not
            
        }

        [HttpGet("/cup/{id}", Name = "Products_List")]
        public IActionResult GetProduct(int id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        //Search: LostGroup: Id= 1924
        //Register: FoundGroup: Id=1907
        [HttpPost("/findLostGroup")]
        public async Task<IActionResult> FindLostGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {
            List<FaceIdentity> allIdentities = getLocalIdentities(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json");

            string encoded = convertToBase64(file);

            string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);

            HttpResponseMessage response = await mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { 1924 }, encoded, 1);

            string apiResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                FaceIdentity result = searchFace(allIdentities, searchFaceIdentityResponse);
                if (result == null) //Search Failed
                {
                    FaceIdentityInfo faceIdentityInfoResponse = await createIdentity(name+age, encoded, mxFaceIdentityAPI, new List<int> { 1907 });
                    allIdentities.Add(new FaceIdentity
                    {
                        IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                        Image = encoded

                    });

                    //Store JSON
                    string resultJson = JsonConvert.SerializeObject(allIdentities);
                    System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);
                    return Ok(new
                    {
                        message = "",
                        FaceIdentityInfo = faceIdentityInfoResponse
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message="Found",
                        image=result.Image
                    });
                }
            }
            else
            {
                return BadRequest();//Failure in Searching
            }
            return BadRequest();
        }

        //Search: FoundGroup: Id=1907
        //Register: LostGroup: Id= 1924
        [HttpPost("/findFoundGroup")]
        public async Task<IActionResult> FindFoundGroup([FromForm] IFormFile file, [FromForm] string name, [FromForm] int age, [FromForm] string city, [FromForm] string details, [FromForm] string postType)
        {

            List<FaceIdentity> allIdentities = getLocalIdentities(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json");

            string encoded = convertToBase64(file);

            string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);

            HttpResponseMessage response = await mxFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int> { 1907 }, encoded, 1);

            string apiResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                SearchFaceIdentityResponse searchFaceIdentityResponse = JsonConvert.DeserializeObject<SearchFaceIdentityResponse>(apiResponse);
                FaceIdentity result = searchFace(allIdentities, searchFaceIdentityResponse);
                if (result == null) //Search Failed
                {
                    FaceIdentityInfo faceIdentityInfoResponse = await createIdentity(name + age, encoded, mxFaceIdentityAPI, new List<int> { 1924 });
                    allIdentities.Add(new FaceIdentity
                    {
                        IdentityId = Convert.ToInt32(faceIdentityInfoResponse.FaceIdentityId),
                        Image = encoded

                    });

                    //Store JSON
                    string resultJson = JsonConvert.SerializeObject(allIdentities);
                    System.IO.File.WriteAllText(@"C:\Users\munee\Desktop\FYP\IFound\backend2\IFoundBackend\IFoundBackend\localDatabase\data.json", resultJson);
                    return Ok(new
                    {
                        message = "",
                        FaceIdentityInfo = faceIdentityInfoResponse
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "Found",
                        image = result.Image
                    });
                }
            }
            else
            {
                return BadRequest();//Failure in Searching
            }
            return BadRequest();
        }



        public List<FaceIdentity> getLocalIdentities(string path)
        {
            string json = System.IO.File.ReadAllText(path);
            List<FaceIdentity> IdentityList = JsonConvert.DeserializeObject<List<FaceIdentity>>(json);
            return IdentityList;
        }

        public string convertToBase64(IFormFile file)
        {
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
            return encoded;
        }

        public FaceIdentity searchFace(List<FaceIdentity> Identity_List, SearchFaceIdentityResponse searchFaceIdentityResponse)
        {
            foreach (var searchedIdentities in searchFaceIdentityResponse.SearchedIdentities)
            {
                foreach (var item in searchedIdentities.identityConfidences)
                {
                    if (item.confidence > 70)
                    {
                        FaceIdentity localIdentity = Identity_List.Find(i => i.IdentityId == item.identity.IdentityId);
                        return localIdentity;
                    }
                }
            }
            return null;
        }

        public async Task<FaceIdentityInfo> createIdentity(string externalId,string encoded, MXFaceIdentityAPI mxFaceIdentityAPI,List<int> groups)
        {
            //Enter post to Group "Found"
            HttpResponseMessage response2 = await mxFaceIdentityAPI.CreateFaceIdentity(groups, externalId, encoded);

            string apiResponse2 = await response2.Content.ReadAsStringAsync();
            if (response2.StatusCode == HttpStatusCode.OK)
            {
                FaceIdentityInfo faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse2);
                return faceIdentityInfoResponse;
               
            }
            return null;
        }

        
    }
    
}