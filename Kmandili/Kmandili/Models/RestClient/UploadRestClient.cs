using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Plugin.Connectivity;

namespace Kmandili.Models.RestClient
{
    class UploadRestClient
    {
        protected async Task<bool> CheckConnection()
        {
            if (CrossConnectivity.Current.IsConnected) return true;
            await App.Current.MainPage.DisplayAlert("Erreur", "Connection Lost", "Ok");
            return false;
        }

        public async Task<bool> Upload(Stream stream, string FileName)
        {
            if (!(await CheckConnection())) return false;
            HttpClient client = new HttpClient();
            var imageStream = new StreamContent(stream);
            var multi = new MultipartContent();
            multi.Add(imageStream);
            try
            {

                var response = await client.PostAsync(App.ServerURL + "api/Uploads/" + FileName, multi);
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

        public async Task<bool> Delete(string fileName)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            string URL = App.ServerURL + "api/Uploads/" + fileName;
            try
            {
                var response = await httpClient.DeleteAsync(URL);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                await
                    App.Current.MainPage.DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur", "Ok");
                return false;
            }
        }
    }
}
