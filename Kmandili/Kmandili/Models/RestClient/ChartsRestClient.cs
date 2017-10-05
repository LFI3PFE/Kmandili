using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmandili.Helpers;

namespace Kmandili.Models.RestClient
{
    class ChartsRestClient
    {

        public async Task<string> GetChartView(string url)
        {
            if (!(await App.CheckConnection()) || (App.TokenExpired())) return "";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            var json = await httpClient.GetStringAsync(url);
            return json;
        }
    }
}
