using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class OrderRestClient : RestClient<Order>
    {
        public async Task<List<Order>> GetAsyncByUserID(int id)
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/ordersByUserID/" + id);

                var taskModels = JsonConvert.DeserializeObject<List<Order>>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }

        }

        public async Task<List<Order>> GetAsyncByPastryShopID(int id)
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/ordersByPastryShopID/" + id);

                var taskModels = JsonConvert.DeserializeObject<List<Order>>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> MarkAsSeenUser(int id)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();

            var result = await httpClient.PutAsync(App.ServerURL + "api/markAsSeenUser/" + id, null);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> MarkAsSeenPastryShop(int id)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();

            var result = await httpClient.PutAsync(App.ServerURL + "api/markAsSeenPastryShop/" + id, null);

            return result.IsSuccessStatusCode;
        }
    }
}
