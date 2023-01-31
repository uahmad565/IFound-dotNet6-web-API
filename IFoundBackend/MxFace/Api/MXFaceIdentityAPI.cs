using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IFoundBackend.MxFace
{
    public class MXFaceIdentityAPI
    {
        private string _apiUrl;
        private string _subscripptionKey;

        public MXFaceIdentityAPI(string apiUrl, string subscripptionKey)
        {
            _apiUrl = apiUrl;
            _subscripptionKey = subscripptionKey;
        }

        public async Task<HttpResponseMessage> CreateFaceIdentity(List<int> groupIds, string encoded, string externalID, int confidenceThreshold, int qualityThreshold = 80)
        {
            using (var httpClient = new HttpClient())
            {

                CreateFaceIdentityRequest request = new CreateFaceIdentityRequest
                {
                    ConfidenceThreshold = confidenceThreshold,
                    Encoded_Image = encoded,
                    GroupIds = groupIds,
                    externalId = externalID,
                    QualityThreshold=qualityThreshold
                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("FaceIdentity", httpContent);
                return response;     
            }
        }
        public async Task GetFaceIdentityInfoByFaceIdentity(int faceIdentityId)
        {
            FaceIdentityInfo faceIdentityInfoResponse = new FaceIdentityInfo();
            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.GetAsync("FaceIdentity/" + faceIdentityId);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    faceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
                    Console.WriteLine(":::::::::::::::GetFaceIdentity Information by IdentityId - START:::::::::::::::::::::::::::::\n");

                    Console.WriteLine("faceIdentityInfoResponse.FaceIdentityId: {0}, faceIdentityInfoResponse.CreatedDate: {1}, faceIdentityInfoResponse.UpdatedDate: {2}",
                        faceIdentityInfoResponse.FaceIdentityId, faceIdentityInfoResponse.CreatedDate, faceIdentityInfoResponse.UpdatedDate);
                    foreach (var face in faceIdentityInfoResponse.Faces)
                    {
                        Console.WriteLine("faceIdentityInfoResponse.FaceID : {0}", face.FaceId);
                    }
                    foreach (var group in faceIdentityInfoResponse.Groups)
                    {
                        Console.WriteLine("faceIdentityInfoResponse.GroupId : {0}", group);
                    }


                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::GetFaceIdentity Information by IdentityId - END:::::::::::::::::::::::::::::\n");
            }
        }

        public async Task<string> AddFaceInFaceIdentity(int faceIdentityId)
        {
            FaceInfo faceInfoResponse = new FaceInfo();
            using (var httpClient = new HttpClient())
            {
                AddFaceRequest request = new AddFaceRequest()
                {
                    Encoded_Image = Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Leonardo.jpg")),
                    ConfidenceThreshold = 0
                };
                string jsonRequest = JsonConvert.SerializeObject(request);

                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("FaceIdentity/" + faceIdentityId + "/face", httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine(":::::::::::::::Add face into existing Identity - START:::::::::::::::::::::::::::::\n");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    faceInfoResponse = JsonConvert.DeserializeObject<FaceInfo>(apiResponse);
                    Console.WriteLine("faceInfoResponse.FaceId: {0}, faceInfoResponse.CreatedDate: {1}",
                        faceInfoResponse.FaceId, faceInfoResponse.CreatedDate);

                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Add face into existing Identity - END:::::::::::::::::::::::::::::\n");
            }
            return faceInfoResponse.FaceId;
        }

        public async Task GetFacesByFaceIdentity(int faceIdentityId)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.GetAsync("FaceIdentity/" + faceIdentityId + "/faces");
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GetFaceResponse facesResponse = JsonConvert.DeserializeObject<GetFaceResponse>(apiResponse);
                    Console.WriteLine(":::::::::::::::Get Faces By FaceIdentityId - START:::::::::::::::::::::::::::::\n");
                    foreach (var face in facesResponse.Faces)
                    {
                        Console.WriteLine("face.FaceId: {0}, face.CreatedDate: {1}",
                        face.FaceId, face.CreatedDate);
                    }
                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Get Faces By FaceIdentityId - END:::::::::::::::::::::::::::::\n");
            }
        }

        public async Task GetFaceIndentityByGroupId(int GroupId)
        {
            int offset = 0;
            int limit = 100;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.GetAsync("FaceIdentity?groupId=" + GroupId + "&offset=" + offset + "&limit=" + limit);
                string apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(":::::::::::::::Get Face Indentity By GroupId - START:::::::::::::::::::::::::::::\n");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FaceIdentityResponse faceIdentityResponse = JsonConvert.DeserializeObject<FaceIdentityResponse>(apiResponse);

                    Console.WriteLine("faceIdentityResponse.TotalFaceIdentities :{0}", faceIdentityResponse.TotalFaceIdentities);
                    foreach (var faceIdentity in faceIdentityResponse.FaceIdentities)
                    {

                        Console.WriteLine("faceIdentity.CreatedDate : {0}, faceIdentity.UpdatedDate : {1}, faceIdentity.FaceIdentityId : {2}",
                             faceIdentity.CreatedDate, faceIdentity.UpdatedDate, faceIdentity.FaceIdentityId);

                        foreach (var face in faceIdentity.Faces)
                        {
                            Console.WriteLine("face.FaceId: {0}, face.CreatedDate: {1}",
                            face.FaceId, face.CreatedDate);
                        }
                        foreach (var group in faceIdentity.Groups)
                        {
                            Console.WriteLine("group.GroupId : {0}", group);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Get Face Indentity By GroupId - END:::::::::::::::::::::::::::::\n");
            }
        }

        public async Task UpdateFaceIdentityGroup(int faceIdentityId, int addGroupId, int DeleteGroupId)
        {
            using (var httpClient = new HttpClient())
            {
                UpdateGroupRequest request = new UpdateGroupRequest()
                {
                    AddGroupIds = new List<int>(),
                    DeleteGroupIds = new List<int>()
                };
                request.AddGroupIds.Add(857);
                request.DeleteGroupIds.Add(858);
                string jsonRequest = JsonConvert.SerializeObject(request);

                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync("FaceIdentity/" + faceIdentityId + "/updateGroup", httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine(":::::::::::::::Update Group By Face Identity Id - START:::::::::::::::::::::::::::::\n");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FaceIdentityInfo updateFaceIdentityInfoResponse = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);

                    Console.WriteLine("updatefaceIdentityInfoResponse.CreatedDate : {0}, updatefaceIdentityInfoResponse.UpdatedDate : {1}," +
                        "updatefaceIdentityInfoResponse.FaceIdentityId : {2}"
                            , updateFaceIdentityInfoResponse.CreatedDate, updateFaceIdentityInfoResponse.UpdatedDate, updateFaceIdentityInfoResponse.FaceIdentityId);

                    foreach (var face in updateFaceIdentityInfoResponse.Faces)
                    {
                        Console.WriteLine("face.FaceId: {0}, face.CreatedDate: {1}",
                        face.FaceId, face.CreatedDate);
                    }
                    foreach (var group in updateFaceIdentityInfoResponse.Groups)
                    {
                        Console.WriteLine("group.GroupId : {0}", group);
                    }


                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Update Group By Face Identity Id - END:::::::::::::::::::::::::::::\n");
            }
        }
        public async Task<HttpResponseMessage> SearchFaceIdentityInGroup(List<int> groupId, string encoded,int limit,int qualityThreshold=80)
        {
            using (var httpClient = new HttpClient())
            {
                SearchFaceIdentity request = new SearchFaceIdentity()
                {
                    GroupIds = groupId,
                    Encoded_Image = encoded,
                    Limit = limit,
                    QualityThreshold=qualityThreshold
                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("FaceIdentity/search", httpContent);
                return response;
                
            }
        }

        public async Task DeleteFaceFromFaceIdentity(int faceId, int faceIdentityId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.DeleteAsync("FaceIdentity/" + faceIdentityId + "/faces/" + faceId);
                string apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(":::::::::::::::Delete Face By Face Identity ID - START:::::::::::::::::::::::::::::\n");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FaceInfo faceInfoResponse = JsonConvert.DeserializeObject<FaceInfo>(apiResponse);
                    Console.WriteLine("faceInfoResponse.FaceId: {0} faceInfoResponse.CreatedDate: {1} ", faceInfoResponse.FaceId, faceInfoResponse.CreatedDate);
                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Delete Face By Face Identity ID - END:::::::::::::::::::::::::::::\n");
            }
        }

        public async Task DeleteFaceIdentity(int faceIdentityId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.DeleteAsync("FaceIdentity/" + faceIdentityId);
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine(":::::::::::::::Delete Face Identity - START:::::::::::::::::::::::::::::\n");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FaceIdentityInfo deleteFaceIdentityInfo = JsonConvert.DeserializeObject<FaceIdentityInfo>(apiResponse);
                    if (string.IsNullOrEmpty(deleteFaceIdentityInfo.ErrorMessage))
                    {
                        Console.WriteLine("face.Identity : {0} deleted successfully.", faceIdentityId);
                    }
                    else
                    {
                        Console.WriteLine("Error message : {0}", deleteFaceIdentityInfo.ErrorMessage);
                    }

                }
                else
                {
                    Console.WriteLine("Error message : {0}, StatusCode : {1}", response.Content, response.StatusCode);
                }
                Console.WriteLine(":::::::::::::::Delete Face By Face Identity ID - END:::::::::::::::::::::::::::::\n");
            }
        }
    }
}

