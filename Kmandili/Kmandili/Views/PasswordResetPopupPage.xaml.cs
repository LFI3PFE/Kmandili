using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordResetPopupPage
	{
	    private readonly string _email;

		public PasswordResetPopupPage (string email)
		{
            BackgroundColor = Color.FromHex("#CC000000");
		    _email = email;
            InitializeComponent ();
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (Password.Text != SencondPassword.Text)
	        {
	            await DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "Ok");
	            Password.Text = "";
	            SencondPassword.Text = "";
	            Password.Focus();
                return;
	        }
            PasswordResetRestClient passwordRc = new PasswordResetRestClient();
	        try
	        {
                if (!(await passwordRc.PutAsync(_email, Password.Text)))
                {
                    var choice = await
                        DisplayAlert("Erreur", "Une erreur s'est produite lors de la mise à jour du mot de passe!", "Ressayer",
                            "Annuler");
                    if (choice)
                    {
                        ComfirmTapped(sender, e);
                    }
                    else
                    {
                        await PopupNavigation.PopAllAsync();
                        return;
                    }
                }
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
	        await DisplayAlert("Succès", "Mot de passe mis à jour avec succés", "Ok");
	        await PopupNavigation.PopAsync();
	    }
	}
}
