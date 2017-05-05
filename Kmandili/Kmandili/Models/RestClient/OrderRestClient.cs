using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class OrderRestClient : RestClient<Order>
    {
        public async Task<List<Order>> GetAsyncByUserID(int id)
        {
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

        public async Task<List<Order>> MarkAsSeenUser(int id)
        {
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/markAsSeenUser/" + id);

                var taskModels = JsonConvert.DeserializeObject<List<Order>>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<Order>> MarkAsSeenPastryShop(int id)
        {
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/markAsSeenPastryShop/" + id);

                var taskModels = JsonConvert.DeserializeObject<List<Order>>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
