using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<bool> SendOrderEmail(int id)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(App.ServerURL + "api/sendOrderEmail/" + id);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> SendCancelOrderEmail(int id)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(App.ServerURL + "/api/sendCanelOrderEmail/" + id);

            return result.IsSuccessStatusCode;
        }
    }
}
