using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class PastryShopRestClient : RestClient<PastryShop>
    {
        public async Task<PastryShop> GetAsyncByEmailAndPass(string email, string password)
        {
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + email + "/" + password);
                var taskModels = JsonConvert.DeserializeObject<PastryShop>(json);

                return taskModels;
            }catch(HttpRequestException)
            {
                return null;
            }
        }

        public async Task<PastryShop> GetAsyncByEmail(string email)
        {
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + "byemail/" + email + "/");
                var taskModels = JsonConvert.DeserializeObject<PastryShop>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> PutAsyncCategories(int id, PastryShop pastryShop)
        {
            var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(pastryShop);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PutAsync(App.ServerURL + "api/PastryShops/Categories/" + id, httpContent);

            return result.IsSuccessStatusCode;
        }
    }
}
