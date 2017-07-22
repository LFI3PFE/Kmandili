using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Plugin.Connectivity;

namespace Kmandili.Models.RestClient
{
    class EmailRestClient
    {
        protected async Task<bool> CheckConnection()
        {
            if (CrossConnectivity.Current.IsConnected) return true;
            await App.Current.MainPage.DisplayAlert("Erreur", "Email not sent due to connection error", "Ok");
            return false;
        }

        public async Task<string> SendPasswordRestCode(string email)
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();

            string WebServiceUrl = App.ServerURL + "api/SendPasswordRestCode/" + email + "/";
            try
            {
                var result = await httpClient.PostAsync(WebServiceUrl, null);
                var taskModels = JsonConvert.DeserializeObject<string>(await result.Content.ReadAsStringAsync());
                return taskModels;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return "";
            }
        }

        public async Task<string> SendEmailVerification(string email)
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();

            string WebServiceUrl = App.ServerURL + "api/sendEmailVerificationCode/" + email + "/";
            try
            {
                var result = await httpClient.PostAsync(WebServiceUrl, null);
                var taskModels = JsonConvert.DeserializeObject<string>(await result.Content.ReadAsStringAsync());
                return taskModels;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return "";
            }
        }

        public async Task<bool> SendOrderEmail(int id)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var result = await httpClient.GetAsync(App.ServerURL + "api/sendOrderEmail/" + id);
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

        public async Task<bool> SendCancelOrderEmail(int id)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var result = await httpClient.GetAsync(App.ServerURL + "/api/sendCanelOrderEmail/" + id);
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
    }
}
