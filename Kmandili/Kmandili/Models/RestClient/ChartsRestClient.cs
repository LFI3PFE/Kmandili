using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace Kmandili.Models.RestClient
{
    class ChartsRestClient
    {
        protected async Task<bool> CheckConnection()
        {
            if (CrossConnectivity.Current.IsConnected) return true;
            await App.Current.MainPage.DisplayAlert("Erreur", "Email not sent due to connection error", "Ok");
            return false;
        }

        public async Task<string> GetChartView(string url)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return "";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetStringAsync(url);
                return json;
            }
            catch (HttpRequestException)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return "";
            }
        }
    }
}
