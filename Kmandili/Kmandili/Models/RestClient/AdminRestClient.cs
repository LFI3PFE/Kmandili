using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models.LocalModels;
using Newtonsoft.Json;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class AdminRestClient
    {
        public async Task<Admin> GetAdmin()
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return default(Admin);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(App.ServerUrl + "api/Admin");
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var admin = JsonConvert.DeserializeObject<Admin>(await json.Content.ReadAsStringAsync());
                return admin;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<bool> UpdateAdmin(string userName, string password)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.PutAsync(App.ServerUrl + "api/Admin/" + userName + "/" + password, null);
            return result.IsSuccessStatusCode;
        }
    }
}
