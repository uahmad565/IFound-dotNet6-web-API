using MXFaceAPIOneToNCall.Model.Group;
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
    public class MXFaceGroupAPI
    {
        private string _apiUrl;
        private string _subscripptionKey;

        public MXFaceGroupAPI(string apiUrl, string subscripptionKey)
        {
            _apiUrl = apiUrl;
            _subscripptionKey = subscripptionKey;
        }

        #region Group

        public async Task<int> CreateGroup(string groupName)
        {
            GroupResponse groupResponse = new GroupResponse();
            using (var httpClient = new HttpClient())
            {
                CreateGroupRequest request = new CreateGroupRequest
                {
                    GroupName = groupName
                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("Group", httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    groupResponse = JsonConvert.DeserializeObject<GroupResponse>(apiResponse);
                    Console.WriteLine("groupResponse.GroupId: {0}, groupResponse.GroupName: {1}, groupResponse.CreatedDate: {2}", groupResponse.GroupId, groupResponse.GroupName, groupResponse.CreatedDate);

                }
                else
                {
                    Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                }

            }
            return groupResponse.GroupId.Value;

        }

        public async Task SearchGroupByName(string GroupName)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.GetAsync("Group?groupName=" + GroupName);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GetGroupsResponse searchGroupsResponse = JsonConvert.DeserializeObject<GetGroupsResponse>(apiResponse);
                    foreach (var grpResponse in searchGroupsResponse.Groups)
                    {
                        Console.WriteLine("searchGroupsResponse.GroupId: {0}, searchGroupsResponse.GroupName: {1}, searchGroupsResponse.CreatedDate: {2}, searchGroupsResponse.UpdatedDate: {3}",
                            grpResponse.GroupId, grpResponse.GroupName, grpResponse.CreatedDate, grpResponse.UpdatedDate);
                    }
                }
                else
                {
                    Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                }
            }

        }
        public async Task SearchGroupByID(int GroupId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.GetAsync("Group/" + GroupId);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GroupResponse groupResponse = JsonConvert.DeserializeObject<GroupResponse>(apiResponse);
                    Console.WriteLine("groupResponse.GroupId: {0}, groupResponse.GroupName: {1}, groupResponse.CreatedDate: {2}, groupResponse.UpdatedDate: {3}", groupResponse.GroupId,
                        groupResponse.GroupName, groupResponse.CreatedDate, groupResponse.UpdatedDate);
                }
                else
                {
                    Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                }
            }

        }

        public async Task UpdateGroupByGroupID(string updatedGroupName, int GroupId)
        {
            using (var httpClient = new HttpClient())
            {
                CreateGroupRequest updateGroupRequest = new CreateGroupRequest
                {
                    GroupName = updatedGroupName   // Group name to update exsiting group
                };
                string jsonRequest = JsonConvert.SerializeObject(updateGroupRequest);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync("Group?id=" + GroupId, httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GroupResponse groupResponse = JsonConvert.DeserializeObject<GroupResponse>(apiResponse);
                    Console.WriteLine("groupResponse.GroupId: {0}, groupResponse.GroupName: {1}, groupResponse.CreatedDate: {2}, groupResponse.UpdatedDate: {3}", groupResponse.GroupId,
                        groupResponse.GroupName, groupResponse.CreatedDate, groupResponse.UpdatedDate);
                }
                else
                {
                    Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                }
            }

        }
        public async Task DeleteGroupByGroupID(int GroupId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscripptionKey);
                HttpResponseMessage response = await httpClient.DeleteAsync("Group/" + GroupId);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GroupResponse groupResponse = JsonConvert.DeserializeObject<GroupResponse>(apiResponse);
                    Console.WriteLine("groupResponse.GroupId: {0}, groupResponse.GroupName: {1}, groupResponse.CreatedDate: {2}, groupResponse.UpdatedDate: {3}", groupResponse.GroupId,
                        groupResponse.GroupName, groupResponse.CreatedDate, groupResponse.UpdatedDate);
                }
                else
                {
                    Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                }
            }

        }
        #endregion

        #region Face Indentity

        #endregion
    }
}
