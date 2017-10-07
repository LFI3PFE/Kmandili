using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;

namespace Kmandili.Models.RestClient
{
    class UploadRestClient
    {

        public async Task<bool> Upload(Stream stream, string FileName)
        {
            if (!(await App.CheckConnection())) return false;
            HttpClient client = new HttpClient();
            var imageStream = new StreamContent(stream);
            var multi = new MultipartContent();
            multi.Add(imageStream);
            var response = await client.PostAsync(App.ServerUrl + "api/Uploads/" + FileName, multi);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(string fileName)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            string URL = App.ServerUrl + "api/Uploads/" + fileName;
            var response = await httpClient.DeleteAsync(URL);
            return response.IsSuccessStatusCode;
        }
    }
}
