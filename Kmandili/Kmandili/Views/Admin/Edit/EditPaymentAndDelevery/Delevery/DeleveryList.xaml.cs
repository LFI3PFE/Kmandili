using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeleveryList : ContentPage
	{
	    private List<DeleveryMethod> deleveryMethods; 

		public DeleveryList ()
		{
			InitializeComponent ();
            Load();
		}

	    public async void Load()
	    {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var deleveryMethodRestClient = new RestClient<DeleveryMethod>();
            try
            {
                List.ItemsSource = deleveryMethods = await deleveryMethodRestClient.GetAsync();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                return;
            }
            ListLayout.IsVisible = true;
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
        }

	    private async void SelectedNot(object sender, ItemTappedEventArgs e)
	    {
	        List.SelectedItem = null;
	        var choix = await
	            DisplayActionSheet("Choisir une action", "Annuler", null, "Renomer la methode",
	                "Changer les méthodes de paiement");
	        switch (choix)
	        {
                case "Renomer la methode":
	                await PopupNavigation.PushAsync(new EditDeleveryMethod(((e.Item) as DeleveryMethod), this));
	                break;
                case "Changer les méthodes de paiement":
	                break;
	        }

	    }

	    private void RemoveDeleveryMethod(object sender, EventArgs e)
	    {
	        
	    }
    }
}
