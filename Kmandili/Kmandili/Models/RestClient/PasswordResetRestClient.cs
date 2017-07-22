using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace Kmandili.Models.RestClient
{
    class PasswordResetRestClient
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

        public async Task<bool> PutAsync(string email, string newPassword)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();
            try
            {
                var result = await httpClient.PutAsync(App.ServerURL + "api/passwords/" + email + "/" + newPassword + "/", null);

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
