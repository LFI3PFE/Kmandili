using System.Net.Http;
using System.Threading.Tasks;

namespace Kmandili.Models.RestClient
{
    class PasswordResetRestClient
    {

        public async Task<bool> PutAsync(string email, string newPassword)
        {
            if (!(await App.CheckConnection())) return false;
            var httpClient = new HttpClient();
            var result = await httpClient.PutAsync(App.ServerUrl + "api/passwords/" + email + "/" + newPassword + "/", null);
            return result.IsSuccessStatusCode;
        }
    }
}
