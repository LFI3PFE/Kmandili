using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class OrderRestClient : RestClient<Order>
    {
        public async Task<List<Order>> GetAsyncByUserId(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(App.ServerUrl + "api/ordersByUserID/" + id);
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<List<Order>>(await json.Content.ReadAsStringAsync());

                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<List<Order>> GetAsyncByPastryShopId(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(App.ServerUrl + "api/ordersByPastryShopID/" + id);
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<List<Order>>(await json.Content.ReadAsStringAsync());

                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<bool> MarkAsSeenUser(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.PutAsync(App.ServerUrl + "api/markAsSeenUser/" + id, null);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> MarkAsSeenPastryShop(int id)
        {
            if (!(await App.CheckConnection()) || App.TokenExpired()) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.PutAsync(App.ServerUrl + "api/markAsSeenPastryShop/" + id, null);
            return result.IsSuccessStatusCode;
        }
    }
}
