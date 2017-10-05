using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class PastryShopRestClient : RestClient<PastryShop>
    {

        public async Task<PastryShop> GetAsyncByEmail(string email)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl + "byemail/" + email + "/");
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<PastryShop>(await json.Content.ReadAsStringAsync());
                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<bool> PutAsyncCategories(int id, PastryShop pastryShop)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(pastryShop);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(App.ServerURL + "api/PastryShops/Categories/" + id, httpContent);

            return result.IsSuccessStatusCode;
        }
    }
}
