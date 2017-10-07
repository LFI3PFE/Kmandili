using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordResetCodeVerification
	{
	    private readonly string _email;
	    private string _code = "";
	    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public PasswordResetCodeVerification(string email)
		{
            BackgroundColor = Color.FromHex("#CC000000");
		    _email = email;
            InitializeComponent ();
		    SendCode();
		}

	    private async void SendCode()
	    {
	        EmailRestClient emailRc = new EmailRestClient();
	        try
	        {
                _code = await emailRc.SendPasswordRestCode(_email);
            }
	        catch (HttpRequestException)
	        {
	            await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
	        }
            CodeTimeExceeded();
        }

        private async void CodeTimeExceeded()
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
                await DisplayAlert("Erreur", "Délai d'attente dépassé!", "Ok");
                await PopupNavigation.PopAsync();
            }
            catch (TaskCanceledException)
            {
            }
        }

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
            if (string.IsNullOrEmpty(Code.Text)) return;
            if (Code.Text.Length == 6 && _code == Code.Text)
            {
                _cancellationTokenSource.Cancel();
                await PopupNavigation.PopAsync();
                await PopupNavigation.PushAsync(new PasswordResetPopupPage(_email));
            }
            else
            {
                Code.Text = "";
                Code.Placeholder = "Code Invalide";
                Code.PlaceholderColor = Color.Red;
            }
        }

	    private void Code_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
            Code.Placeholder = "Code";
            Code.PlaceholderColor = Color.LightGray;
        }
	}
}
