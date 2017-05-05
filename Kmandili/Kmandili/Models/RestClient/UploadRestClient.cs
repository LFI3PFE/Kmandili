using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class UploadRestClient
    {
        public async Task<bool> Upload(Stream stream, string FileName)
        {
            HttpClient client = new HttpClient();
            var imageStream = new StreamContent(stream);
            var multi = new MultipartContent();
            multi.Add(imageStream);
            var response = await client.PostAsync(App.ServerURL + "api/Uploads/" + FileName, multi);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(string fileName)
        {
            var httpClient = new HttpClient();

            string URL = App.ServerURL + "api/Uploads/" + fileName;
            var response = await httpClient.DeleteAsync(URL);

            return response.IsSuccessStatusCode;
        }
    }
}
