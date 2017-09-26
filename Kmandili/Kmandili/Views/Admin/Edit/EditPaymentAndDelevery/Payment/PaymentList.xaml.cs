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
	    private ToolbarItem addToolbarItem;

		public PaymentList ()
		{
			InitializeComponent ();
            List.SeparatorVisibility = SeparatorVisibility.None;
            addToolbarItem = new ToolbarItem()
            {
                Icon = "plus.png",
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            addToolbarItem.Clicked += AddToolbarItem_Clicked;
            ToolbarItems.Add(addToolbarItem);
            Load();
		}

        private async void AddToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new AddPaymentPopupPage(this));
        }

        public async void Load()
	    {
	        ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var paymentMethodRestClient = new RestClient<Models.Payment>();
            try
            {
                List.ItemsSource = await paymentMethodRestClient.GetAsync();
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

	    private async void RemovePaymentMethod(object sender, EventArgs e)
	    {
            var choix = await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer cette méthode de paiement?", "Oui", "Annuler");
            if (!choix) return;
            var id = (Int32.Parse((((sender as Image).Parent as StackLayout).Children[0] as Label).Text));
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var paymentRC = new RestClient<Models.Payment>();
            var cat = await paymentRC.GetAsyncById(id);
            if (cat.Orders.Any() ||
                cat.PastryDeleveryPayments.Any())
            {
                await PopupNavigation.PopAllAsync();
                await
                        DisplayAlert("Erreur",
                            "Impossible de supprimer cette méthode de paiement, une ou plusieurs méthodes de livraison et / ou commandes l'utilisent",
                            "Ok");
                return;
            }
            try
            {
                if (!(await paymentRC.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la suppression de la méthode de paiement, veuillez réessayer plus tard.",
                            "Ok");
                    return;
                }
                Load();
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
            await PopupNavigation.PopAllAsync();
        }

    }
}
