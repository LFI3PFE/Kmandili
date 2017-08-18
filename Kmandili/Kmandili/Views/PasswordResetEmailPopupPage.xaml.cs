using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordResetEmailPopupPage : PopupPage
	{
		public PasswordResetEmailPopupPage()
		{
            BackgroundColor = Color.FromHex("#CC000000");
            InitializeComponent ();
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (string.IsNullOrEmpty(Email.Text)) return;
	        if (!App.isValidEmail(Email.Text.ToLower()))
	        {
	            await DisplayAlert("Erreur", "Adresse email invalide", "Ok");
	            return;
	        }
	        Email.IsEnabled = false;
	        ComfirmLabel.IsEnabled = false;
            ComfirmLabel.TextColor = Color.DarkGray;
	        Indicator.IsVisible = true;
            Indicator.IsRunning = true;
	        try
	        {
                if (!(await App.GetEmailExist(Email.Text.ToLower())))
                {
                    await DisplayAlert("Erreur", "Utilisateur inexistant!", "Ok");
                    await PopupNavigation.PopAsync();
                    return;
                }
                await PopupNavigation.PopAsync();
                await PopupNavigation.PushAsync(new PasswordResetCodeVerification(Email.Text.ToLower()));
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
            }
        }
	}
}
