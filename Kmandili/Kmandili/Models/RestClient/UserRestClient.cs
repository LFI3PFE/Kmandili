﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;

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
                var json = await httpClient.GetStringAsync(WebServiceUrl + "byemail/" + email + "/");

                var taskModels = JsonConvert.DeserializeObject<User>(json);

                return taskModels;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message == "404 (Not Found)")
                {
                    return null;
                }
                throw;
            }
        }
    }
}
