using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Newtonsoft.Json;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class RatingRestClient : RestClient<Rating>
    {
        public async Task<Rating> GetAsyncById(int userFk, int pastryShopFk)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return default(Rating);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl + userFk + "/" + pastryShopFk + "/");
                if (json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<Rating>(await json.Content.ReadAsStringAsync());
                return taskModels;
            }catch (WebException) {
                throw new HttpRequestException();
            }
        }

        public async Task<bool> PutAsync(int userFk, int pastryShopFk, Rating rating)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(rating);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(WebServiceUrl + userFk + "/" + pastryShopFk + "/", httpContent);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int userFk, int pastryShopFk)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var response = await httpClient.DeleteAsync(WebServiceUrl + userFk + "/" + pastryShopFk + "/");
            return response.IsSuccessStatusCode;
        }
    }
}
