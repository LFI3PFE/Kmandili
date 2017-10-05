using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Models.LocalModels;
using Newtonsoft.Json;

namespace Kmandili.Models.RestClient
{
    public class AuthorizationRestClient
    {

        public async Task<TokenResponse> AuthorizationLoginAsync(string email, string password)
        {
            if (!(await App.CheckConnection())) return null;
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("UserName", email),
                new KeyValuePair<string, string>("Password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(HttpMethod.Post, App.ServerURL + "token")
            {
                Content = new FormUrlEncodedContent(keyValues)
            };

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
            return tokenResponse;
        }
    }
}
