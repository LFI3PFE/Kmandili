using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordResetCodeVerification : PopupPage
	{
	    private string email;
	    private string code = "";
	    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public PasswordResetCodeVerification(string email)
		{
            BackgroundColor = Color.FromHex("#CC000000");
		    this.email = email;
            InitializeComponent ();
		    SendCode();
		}

	    private async void SendCode()
	    {
	        EmailRestClient emailRC = new EmailRestClient();
	        try
	        {
                code = await emailRC.SendPasswordRestCode(email);
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
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationTokenSource.Token);
                await DisplayAlert("Erreur", "Délai d'attente dépassé!", "Ok");
                await PopupNavigation.PopAsync();
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
            if (string.IsNullOrEmpty(Code.Text)) return;
            if (Code.Text.Length == 6 && code == Code.Text)
            {
                cancellationTokenSource.Cancel();
                await PopupNavigation.PopAsync();
                await PopupNavigation.PushAsync(new PasswordResetPopupPage(email));
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
