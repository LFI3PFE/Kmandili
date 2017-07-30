using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;

namespace Kmandili.Models.RestClient
{
    class PastryShopRestClient : RestClient<PastryShop>
    {
        //public async Task<PastryShop> GetAsyncByEmailAndPass(string email, string password)
        //{
        //    if (!(await CheckConnection()))
        //    {
        //        throw new ConnectionLostException();
        //    }
        //    var httpClient = new HttpClient();
        //    try
        //    {
        //        var json = await httpClient.GetStringAsync(WebServiceUrl + email + "/" + password);
        //        var taskModels = JsonConvert.DeserializeObject<PastryShop>(json);

        //        return taskModels;
        //    }catch(HttpRequestException ex)
        //    {
        //        if (ex.Message == "404 (Not Found)")
        //        {
        //            return null;
        //        }
        //        throw;
        //        //await
        //        //    App.Current.MainPage.DisplayAlert("Erreur",
        //        //        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
        //        //return null;
        //    }
        //}

        public async Task<PastryShop> GetAsyncByEmail(string email)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + "byemail/" + email + "/");
                var taskModels = JsonConvert.DeserializeObject<PastryShop>(json);
                return taskModels;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message == "404 (Not Found)")
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<bool> PutAsyncCategories(int id, PastryShop pastryShop)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
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
