using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models.RestClient;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditAdmin : ContentPage
	{
	    private Models.LocalModels.Admin admin;

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
	            var adminRC = new AdminRestClient();
                admin = await adminRC.GetAdmin();
	            Email.Text = admin.UserName;
	            Password.Text = admin.Password;
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
	        if ((admin.UserName == Email.Text.ToLower()) && (admin.Password == Password.Text)) return;
	        if (!App.isValidEmail(Email.Text.ToLower()))
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
                await PopupNavigation.PushAsync(new EmailVerificationPopupPage(this, Email.Text.ToLower()));
                //EmailVerified();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
	    }

	    public async void EmailVerified()
	    {
            try
            {
                await PopupNavigation.PushAsync(new LoadingPopupPage());
                var adminRC = new AdminRestClient();
                if (!(await adminRC.UpdateAdmin(Email.Text.ToLower(), Password.Text)))
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
                return;
            }
        }

    }
}
