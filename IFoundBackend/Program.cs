using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFoundBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


//static string x(int i)
//{
//    string path = $"C:\\Users\\administrator\\Downloads\\MXFaceAPIOneToNCall\\MXFaceAPIOneToNCall\\MXFaceAPIOneToNCall\\Model\\Images\\Usman\\{i}.jpg";
//    byte[] imageArray = System.IO.File.ReadAllBytes(path);
//    string base64String = Convert.ToBase64String(imageArray);
//    return base64String;
//}

//static async Task Main(string[] args)
//{

//    string encoded = x(1);
//    int testGroupId = 1907;
//    string subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";//your subscription key
//                                                                 //MXFaceGroupAPI mXFaceGroupAPI = new MXFaceGroupAPI(, );
//    MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);
//    //string apiResponse = "{\"faceIdentityId\":2835,\"faces\":[{\"faceId\":\"3023\",\"createdDate\":\"2022-12-25T14:57:40+00:00\"}],\"groups\":[{\"groupId\":1907,\"groupName\":\"testGroup\",\"createdDate\":\"2022-12-18T23:52:01+00:00\",\"updatedDate\":\"2022-12-18T23:52:01+00:00\"}],\"createdDate\":\"2022-12-25T14:57:40+00:00\",\"externalId\":\"prac1\",\"updatedDate\":\"2022-12-25T14:57:40+00:00\"}";
//    //var faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
//    await mxFaceIdentityAPI.CreateFaceIdentity(new List<int> { 1907 }, "usman1", encoded);
//    //await mxFaceIdentityAPI.DeleteFaceIdentity(2828);
//    //await mXFaceGroupAPI.SearchGroupByName("testGroup");



//    //Create FaceIdentities
//    FaceIdentityInfo faceIdentity = new FaceIdentityInfo();


//    //MXFaceIdentityAPI mXFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", subscriptionKey);

//    //Console.WriteLine("\n:::::::::::::::Calling Create Group API - START:::::::::::::::::::::::::::::\n");

//    //string GroupName = "Group-5651"; 
//    //string NewGroup = "new Group-64451";
//    //string UpdateGroupName = "Group-12345-update5651";
//    ////int GroupId = await mXFaceGroupAPI.CreateGroup(GroupName);
//    ////int NewGroupId = await mXFaceGroupAPI.CreateGroup(NewGroup);
//    //int GroupId = 1905;
//    //int NewGroupId = 1906;
//    //string ExternalId = "ext-1336";
//    //int? FaceIdentityId = await mXFaceIdentityAPI.CreateFaceIdentity(new List<int>() { GroupId }, ExternalId);
//    //await mXFaceIdentityAPI.GetFaceIdentityInfoByFaceIdentity(FaceIdentityId.Value);

//    //string FaceId = await mXFaceIdentityAPI.AddFaceInFaceIdentity(FaceIdentityId.Value);

//    //await mXFaceIdentityAPI.GetFacesByFaceIdentity(FaceIdentityId.Value);
//    //await mXFaceIdentityAPI.GetFaceIndentityByGroupId(GroupId);
//    //await mXFaceIdentityAPI.SearchFaceIdentityInGroup(new List<int>() { GroupId });
//    //await mXFaceIdentityAPI.UpdateFaceIdentityGroup(FaceIdentityId.Value, NewGroupId, GroupId);
//    //await mXFaceIdentityAPI.DeleteFaceFromFaceIdentity(Convert.ToInt32(FaceId), FaceIdentityId.Value);
//    //await mXFaceIdentityAPI.DeleteFaceIdentity(FaceIdentityId.Value);

//    //Console.WriteLine("\n:::::::::::::::Calling Search Group API By GroupName :::::::::::::::::::::::::::::\n");
//    //await mXFaceGroupAPI.SearchGroupByName(GroupName);

//    //Console.WriteLine("\n:::::::::::::::Calling Search Group API By GroupId :::::::::::::::::::::::::::::\n");
//    //await mXFaceGroupAPI.SearchGroupByID(GroupId);

//    //Console.WriteLine("\n:::::::::::::::Calling Update Group API:::::::::::::::::::::::::::::\n");
//    //await mXFaceGroupAPI.UpdateGroupByGroupID(UpdateGroupName, GroupId);

//    //Console.WriteLine("\n:::::::::::::::Calling Delete Group API:::::::::::::::::::::::::::::\n");
//    //await mXFaceGroupAPI.DeleteGroupByGroupID(GroupId);

//    //await mXFaceGroupAPI.DeleteGroupByGroupID(NewGroupId);
//    //Console.ReadKey();
//}