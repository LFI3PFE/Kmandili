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
using Plugin.Connectivity;

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

        protected async Task<bool> CheckConnection()
        {
            while (!CrossConnectivity.Current.IsConnected)
            {
                await App.Current.MainPage.DisplayAlert("Erreur", "Pas de connection internet", "Ressayer");
                return (await CheckConnection());
            }
            return true;
        }

        public async Task<List<T>> GetAsync()
        {
            if (!(await CheckConnection())) return null;
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl);

                var taskModels = JsonConvert.DeserializeObject<List<T>>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<T> GetAsyncById(int id)
        {
            if (!(await CheckConnection())) return default(T);
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync(WebServiceUrl + id);

                var taskModels = JsonConvert.DeserializeObject<T>(json);

                return taskModels;
            }
            catch (HttpRequestException)
            {
                return default(T);
            }
        }
        
        public async Task<T> PostAsync(T t)
        {
            if (!(await CheckConnection())) return default(T);
            var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PostAsync(WebServiceUrl, httpContent);
            var taskModels = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
            return taskModels;
        }

        public async Task<bool> PutAsync(int id, T t)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PutAsync(WebServiceUrl + id, httpContent);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!(await CheckConnection())) return false;
            var httpClient = new HttpClient();

            var response = await httpClient.DeleteAsync(WebServiceUrl + id);

            return response.IsSuccessStatusCode;
        }
    }
}
