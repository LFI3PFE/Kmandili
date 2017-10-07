using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Newtonsoft.Json;

namespace Kmandili.Models.RestClient
{
    class DeleveryMethodRestClient : RestClient<DeleveryMethod>
    {
        public async Task<bool> PutAsyncPayments(int id, DeleveryMethod deleveryMethod)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(deleveryMethod);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(App.ServerUrl + "api/DeleveryMethods/Payments/" + id, httpContent);

            return result.IsSuccessStatusCode;
        }
    }
}
