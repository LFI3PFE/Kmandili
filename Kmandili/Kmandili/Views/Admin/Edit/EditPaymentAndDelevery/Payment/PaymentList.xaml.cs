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

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaymentList : ContentPage
	{
	    private List<Models.Payment> payments; 

		public PaymentList ()
		{
			InitializeComponent ();
		    Load();
		}

	    public async void Load()
	    {
	        ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var paymentMethodRestClient = new RestClient<Models.Payment>();
            try
            {
                List.ItemsSource = payments = await paymentMethodRestClient.GetAsync();
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
	        await PopupNavigation.PushAsync(new EditPayment((e.Item as Models.Payment), this));
	    }

	    private void RemovePaymentMethod(object sender, EventArgs e)
	    {
	        
	    }

    }
}
