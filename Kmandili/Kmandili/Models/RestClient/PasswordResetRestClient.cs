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

        public async Task<bool> PutAsync(string email, string newPassword)
        {
            if (!(await App.CheckConnection())) return false;
            var httpClient = new HttpClient();
            var result = await httpClient.PutAsync(App.ServerURL + "api/passwords/" + email + "/" + newPassword + "/", null);
            return result.IsSuccessStatusCode;
        }
    }
}
