using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Newtonsoft.Json;

namespace Kmandili.Models.RestClient
{
    class RatingRestClient : RestClient<Rating>
    {
        public async Task<Rating> GetAsyncById(int user_fk, int pastryShop_fk)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return default(Rating);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/");

                var taskModels = JsonConvert.DeserializeObject<Rating>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return default(Rating);
            }
        }

        public async Task<bool> PutAsync(int user_fk, int pastryShop_fk, Rating rating)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(rating);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {
                var result = await httpClient.PutAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/", httpContent);

                return result.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int user_fk, int pastryShop_fk)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var response = await httpClient.DeleteAsync(WebServiceUrl + user_fk + "/" + pastryShop_fk + "/");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return false;
            }
        }
    }
}
