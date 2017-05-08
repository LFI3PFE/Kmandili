using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class UserRestClient : RestClient<User>
    {
        public async Task<User> GetAsyncByEmailAndPass(string email, string password)
        {
            if (!(await CheckConnection()))
            {
                throw new ConnectionLostException();
            }
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + email + "/" + password);

                var taskModels = JsonConvert.DeserializeObject<User>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<User> GetAsyncByEmail(string email)
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + "byemail/" + email + "/");

                var taskModels = JsonConvert.DeserializeObject<User>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
