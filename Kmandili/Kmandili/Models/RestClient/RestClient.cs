using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Kmandili.Helpers;
using Plugin.Connectivity;
using System.Net;

namespace Kmandili.Models.RestClient
{
    class RestClient<T>
    {
        //private const string WebServiceUrl = "http://192.168.1.5:300/api/addresses/";
        private string controllerName;
        protected string WebServiceUrl;

        public RestClient()
        {
            if(typeof(T).Name == "Address")
            {
                controllerName = "Addresses";
            }
            else if(typeof(T).Name == "Category")
            {
                controllerName = "Categories";
            }else if (typeof (T).Name == "Status")
            {
                controllerName = typeof (T).Name;
            }
            else
            {
                controllerName = typeof(T).Name+"s";
            }
            WebServiceUrl = App.ServerURL + "api/" + controllerName + "/";
        }

        public async Task<List<T>> GetAsync()
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return null;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl);
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<List<T>>(await json.Content.ReadAsStringAsync());

                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<T> GetAsyncById(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return default(T);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetAsync(WebServiceUrl + id);
                if(json.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(T);
                }else if(json.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException();
                }
                var taskModels = JsonConvert.DeserializeObject<T>(await json.Content.ReadAsStringAsync());

                return taskModels;
            }
            catch (WebException)
            {
                throw new HttpRequestException();
            }
        }
        
        public async Task<T> PostAsync(T t)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return default(T);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PostAsync(WebServiceUrl, httpContent);
            var taskModels = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
            return taskModels;
        }

        public async Task<bool> PutAsync(int id, T t)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await httpClient.PutAsync(WebServiceUrl + id, httpContent);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var response = await httpClient.DeleteAsync(WebServiceUrl + id);

            return response.IsSuccessStatusCode;
        }
    }
}
