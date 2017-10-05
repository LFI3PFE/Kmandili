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
        public async Task<Rating> GetAsyncById(int user_fk, int pastryShop_fk)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return default(Rating);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/");
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

            //try
            //{
            //    var json = await httpClient.GetStringAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/");
            //    var taskModels = JsonConvert.DeserializeObject<Rating>(json);

            //    return taskModels;
            //}
            //catch (HttpRequestException ex)
            //{
            //    if ((ex.InnerException is WebException))
            //    {
            //        var x = ((WebException)ex.InnerException).Status;
            //    }
            //    if (ex.Message == "404 (Not Found)")
            //    {
            //        return null;
            //    }
            //    throw;
            //}
        }

        public async Task<bool> PutAsync(int user_fk, int pastryShop_fk, Rating rating)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(rating);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/", httpContent);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int user_fk, int pastryShop_fk)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var response = await httpClient.DeleteAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/");
            return response.IsSuccessStatusCode;
        }
    }
}
