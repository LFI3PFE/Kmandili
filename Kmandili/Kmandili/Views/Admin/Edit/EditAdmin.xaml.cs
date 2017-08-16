using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                Models.LocalModels.Admin admin = await adminRC.GetAdmin();
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
	        if (!App.isValidEmail(Email.Text.ToLower()))
	        {
	            await DisplayAlert("Erreur", "Email invalide.", "Ok");
	            return;
	        }
            try
            {
                var adminRC = new AdminRestClient();
                if (!(await adminRC.UpdateAdmin(Email.Text.ToLower(), Password.Text)))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour des informations, veuillez réessayer plus tard.", "Ok");
                    return;
                }
                await Navigation.PopAsync();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
	        await PopupNavigation.PopAllAsync();
	    }

    }
}
