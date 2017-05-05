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

        //public async Task<bool> Upload(Stream stream, string FileName)
        //{
        //    HttpClient client = new HttpClient();
        //    var imageStream = new StreamContent(stream);
        //    var multi = new MultipartContent();
        //    multi.Add(imageStream);
        //    var response = await client.PostAsync(App.serverURL + "api/Upload/" + FileName, multi);
        //    return response.IsSuccessStatusCode;
        //}

        //public async Task<List<T>> GetAsyncByMail(string email)
        //{
        //    var httpClient = new HttpClient();

        //    var json = await httpClient.GetStringAsync(WebServiceUrl + "Get" + typeof(T).Name + "ByMail/" + email + "/");

        //    var taskModels = JsonConvert.DeserializeObject<List<T>>(json);

        //    return taskModels;
        //}

        //public async Task<List<T>> GetAsyncOrderByPrice<TRus>(Func<T, TRus> linqFunc, bool isDescending)
        //{
        //    var httpClient = new HttpClient();

        //    var json = await httpClient.GetStringAsync(WebServiceUrl);

        //    List<T> taskModels = JsonConvert.DeserializeObject<List<T>>(json);

        //    if (isDescending)
        //        return taskModels.OrderByDescending(linqFunc).ToList();
        //    else
        //        return taskModels.OrderBy(linqFunc).ToList();
        //}

        //public async Task<List<T>> GetAsyncByName(Func<T, bool> func)
        //{
        //    var httpClient = new HttpClient();

        //    var json = await httpClient.GetStringAsync(WebServiceUrl);

        //    List<T> taskModels = JsonConvert.DeserializeObject<List<T>>(json);

        //    return taskModels.Where(func).ToList();
        //}

        //public async Task<List<T>> GetAsyncOrderBy(string propertyName, bool isDescending)
        //{
        //    var httpClient = new HttpClient();

        //    var s = WebServiceUrl + "Get" + typeof(T).Name + "sByProperty/" + propertyName + "/" + isDescending;

        //    var json = await httpClient.GetStringAsync(s);

        //    var taskModels = JsonConvert.DeserializeObject<List<T>>(json);

        //    return taskModels;
        //}

        //public async Task<List<T>> GetAsyncByName(string KeyWord)
        //{
        //    var httpClient = new HttpClient();
        //    string s;
        //    if (KeyWord.Length == 0 || KeyWord == null)
        //    {
        //        s = WebServiceUrl + "Get" + typeof(T).Name + "sByProperty/Rating/true";
        //    }
        //    else
        //    {
        //        s = WebServiceUrl + "Get" + typeof(T).Name + "sByName/" + KeyWord;
        //    }

        //    var json = await httpClient.GetStringAsync(s);

        //    var taskModels = JsonConvert.DeserializeObject<List<T>>(json);

        //    return taskModels;
        //}


        public async Task<T> PostAsync(T t)
        {
            var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PostAsync(WebServiceUrl, httpContent);
            var taskModels = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
            return taskModels;
            //return result.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync(int id, T t)
        {
            var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PutAsync(WebServiceUrl + id, httpContent);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var httpClient = new HttpClient();

            var response = await httpClient.DeleteAsync(WebServiceUrl + id);

            return response.IsSuccessStatusCode;
        }
    }
}
