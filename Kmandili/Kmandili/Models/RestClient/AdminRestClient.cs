using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models.LocalModels;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace Kmandili.Models.RestClient
{
    class AdminRestClient
    {
        protected async Task<bool> CheckConnection()
        {
            while (!CrossConnectivity.Current.IsConnected)
            {
                await App.Current.MainPage.DisplayAlert("Erreur", "Pas de connection internet", "Ressayer");
                return (await CheckConnection());
            }
            return true;
        }

        public async Task<Admin> GetAdmin()
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return default(Admin);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/Admin");
                var admin = JsonConvert.DeserializeObject<Admin>(json);
                return admin;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message == "404 (Not Found)")
                {
                    return default(Admin);
                }
                throw;
            }
        }

        public async Task<bool> UpdateAdmin(string userName, string password)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var result = await httpClient.PutAsync(App.ServerURL + "api/Admin/" + userName + "/" + password, null);
            return result.IsSuccessStatusCode;
        }
    }
}
