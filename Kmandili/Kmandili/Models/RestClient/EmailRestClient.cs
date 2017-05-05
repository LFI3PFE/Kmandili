using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class EmailRestClient
    {
        public async Task<bool> SendOrderEmail(int id)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(App.ServerURL + "api/sendOrderEmail/" + id);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> SendCancelOrderEmail(int id)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(App.ServerURL + "/api/sendCanelOrderEmail/" + id);

            return result.IsSuccessStatusCode;
        }

        //public async Task<bool> SendOrderStatusChangedEmail(int id)
        //{
        //    var httpClient = new HttpClient();
        //    var result = await httpClient.GetAsync(App.ServerURL + "/api/SendOrderStatusChangedEmail/" + id);

        //    return result.IsSuccessStatusCode;
        //}
    }
}
