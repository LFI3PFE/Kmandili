using System;
using System.Net.Http;
using Kmandili.Helpers;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditAdmin
	{
	    private Models.LocalModels.Admin _admin;

		public EditAdmin ()
		{
			InitializeComponent ();
            Load();
		}

	    private async void Load()
	    {
	        try
	        {
	            await PopupNavigation.PushAsync(new LoadingPopupPage());
	            var adminRc = new AdminRestClient();
                _admin = await adminRc.GetAdmin();
	            Email.Text = _admin.UserName;
	            Password.Text = _admin.Password;
	        }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
	        await PopupNavigation.PopAllAsync();
	    }

	    private async void Update(object sender, EventArgs e)
	    {
	        if ((_admin.UserName == Email.Text.ToLower()) && (_admin.Password == Password.Text)) return;
	        if (!App.IsValidEmail(Email.Text.ToLower()))
	        {
	            await DisplayAlert("Erreur", "Email invalide.", "Ok");
	            return;
	        }
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
            try
            {
                if (await App.GetEmailExist(Email.Text.ToLower()))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Cette adresse email existe déjas", "Ok");
                    return;
                }
                await PopupNavigation.PopAllAsync();
                await PopupNavigation.PushAsync(new AEmailVerificationPopupPage(this, Email.Text.ToLower()));
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
            }
	    }

	    public async void EmailVerified()
	    {
            try
            {
                await PopupNavigation.PushAsync(new LoadingPopupPage());
                var adminRc = new AdminRestClient();
                if (!(await adminRc.UpdateAdmin(Email.Text.ToLower(), Password.Text)))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour des informations, veuillez réessayer plus tard.", "Ok");
                    return;
                }
                Settings.Email = Email.Text.ToLower();
                Settings.Password = Password.Text;
                await PopupNavigation.PopAllAsync();
                await Navigation.PopAsync();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
            }
        }

    }
}
