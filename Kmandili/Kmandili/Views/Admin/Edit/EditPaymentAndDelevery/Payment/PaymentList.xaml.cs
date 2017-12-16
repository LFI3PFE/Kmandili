using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaymentList
	{
	    public PaymentList ()
		{
		    InitializeComponent ();
            List.SeparatorVisibility = SeparatorVisibility.None;
            var addToolbarItem = new ToolbarItem()
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
            var id = (Int32.Parse(((Label) ((StackLayout) ((Image) sender).Parent).Children[0]).Text));
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var paymentRc = new RestClient<Models.Payment>();
            var cat = await paymentRc.GetAsyncById(id);
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
                if (!(await paymentRc.DeleteAsync(id)))
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
