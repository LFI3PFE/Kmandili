using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class UserRestClient : RestClient<User>
    {

        public async Task<User> GetAsyncByEmail(string email)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl + "byemail/" + email + "/");
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<User>(await json.Content.ReadAsStringAsync());

                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }
    }
}
