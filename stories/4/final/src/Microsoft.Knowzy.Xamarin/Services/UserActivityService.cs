using Microsoft.Graph;
using Microsoft.Knowzy.Xamarin;
using Microsoft.Knowzy.Xamarin.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Knowzy.Xamarin.Model.Activity;

namespace Microsoft.Knowzy.Xamarin.Services
{
    public class UserActivityService
    {
        private UserActivityService()
        {

        }

        private static UserActivityService current;

        public static UserActivityService Current => current ?? (current = new UserActivityService());

        public async Task<string> RecordInventoryActivityAndHistoryItemAsync(InventoryModel model)
        {
            var appActivityId = string.Concat("item?id=", model.InventoryId);

            Activity activity = new Activity
            {
                AppActivityId = appActivityId,
                ActivationUrl = $"knowzyinventory:{appActivityId}",
                VisualElements = new VisualInfo { DisplayText = model.Name },
                ActivitySourceHost = "microsoftknowzyweb20171009022902.azurewebsites.net"
            };

            string activitiesUrl = App.GraphClient.Me.AppendSegmentToRequestUrl("activities");
            string activitiesUrlWithId = string.Concat(activitiesUrl, "/", WebUtility.UrlEncode(model.InventoryId));

            var activityEndPointId = await CreateOrUpdateActivity(activity, activitiesUrlWithId);

            var historyId = Guid.NewGuid().ToString();

            HistoryItem historyItem = new HistoryItem
            {
                StartedDateTime = DateTime.UtcNow,
                LastActiveDateTime = DateTime.UtcNow,
                UserTimezone = "America/Los_Angeles"
            };

            string historyUrlWithId = string.Concat(activitiesUrl, "/", activityEndPointId, "/historyItems/", historyId);

            var historyEndPointId = await CreateOrUpdateHistoryItem(historyItem, historyUrlWithId);

            return "Checkpoint created/updated";
        }

        public async Task<string> RemoveInventoryActivityAsync(InventoryModel model)
        {
            var appActivityId = string.Concat("item?id=", model.InventoryId);

            string activitiesUrl = App.GraphClient.Me.AppendSegmentToRequestUrl("activities");
            string activitiesUrlWithId = string.Concat(activitiesUrl, "/", WebUtility.UrlEncode(model.InventoryId));

            return await DeleteActivity(activitiesUrlWithId);
        }

        private async Task<string> DeleteActivity(string activitiesUrlWithId)
        {
            Activity activity = null;
            var response = await CreateCustomGraphRequest(activity, activitiesUrlWithId, HttpMethod.Delete);

            return "Activity Deleted";
        }

        private async Task<string> CreateOrUpdateActivity(Activity activity, string activitiesUrlWithId)
        {
            var response = await CreateCustomGraphRequest(activity, activitiesUrlWithId, HttpMethod.Put);

            var id = response?.Headers?.Location?.Segments?[4];

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Debug.WriteLine($"Activity Updated for {id}");
                    break;
                case HttpStatusCode.Created:
                    Debug.WriteLine($"Activity Created for {id}");
                    break;
            }

            return id;
        }

        private async Task<string> CreateOrUpdateHistoryItem(HistoryItem historyItem, string historyUrlWithId)
        {
            var response = await CreateCustomGraphRequest(historyItem, historyUrlWithId, HttpMethod.Put);

            var id = response?.Headers?.Location?.Segments?[6];

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Debug.WriteLine($"HistoryItem Updated for {id}");
                    break;
                case HttpStatusCode.Created:
                    Debug.WriteLine($"HistoryItem Created for {id}");
                    break;
            }

            return id;
        }

        private async Task<HttpResponseMessage> CreateCustomGraphRequest<T>(T item, string customUrl, HttpMethod method)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, customUrl);
            
            if(method.Method == HttpMethod.Put.Method)
            {
                List<T> containerList = new List<T> { item };
                var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };
                string itemJson = JsonConvert.SerializeObject(containerList, settings);
                var stringContent = new StringContent(itemJson, Encoding.UTF8, "text/json");
                request.Content = stringContent;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationService.Current.TokenForUser);

            HttpResponseMessage response = null;
            try
            {
                response = await App.GraphClient.HttpProvider.SendAsync(request);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }

            return response;
        }
    }
}
