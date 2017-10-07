using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;

namespace Kmandili.Models.RestClient
{
    class EmailRestClient
    {

        public async Task<string> SendPasswordRestCode(string email)
        {
            if (!(await App.CheckConnection())) return null;
            var httpClient = new HttpClient();

            string WebServiceUrl = App.ServerUrl + "api/SendPasswordRestCode/" + email + "/";
            var result = await httpClient.PostAsync(WebServiceUrl, null);
            var taskModels = JsonConvert.DeserializeObject<string>(await result.Content.ReadAsStringAsync());
            return taskModels;
        }

        public async Task<string> SendEmailVerification(string email)
        {
            if (!(await App.CheckConnection())) return null;
            var httpClient = new HttpClient();

            string WebServiceUrl = App.ServerUrl + "api/sendEmailVerificationCode/" + email + "/";
            var result = await httpClient.PostAsync(WebServiceUrl, null);
            var taskModels = JsonConvert.DeserializeObject<string>(await result.Content.ReadAsStringAsync());
            return taskModels;
        }

        public async Task<bool> SendOrderEmail(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.GetAsync(App.ServerUrl + "api/sendOrderEmail/" + id);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> SendCancelOrderEmail(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.GetAsync(App.ServerUrl + "/api/sendCanelOrderEmail/" + id);
            return result.IsSuccessStatusCode;
        }
        
        public async Task<bool> SendCanelOrderEmailByAdmin(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.GetAsync(App.ServerUrl + "/api/sendCanelOrderEmailByAdmin/" + id);
            return result.IsSuccessStatusCode;
        }
    }
}
