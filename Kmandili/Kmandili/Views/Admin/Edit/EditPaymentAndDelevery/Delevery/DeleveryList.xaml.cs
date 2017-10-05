using System;
using System.Linq;
using System.Net.Http;
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
	    private ToolbarItem addToolbarItem;

		public DeleveryList ()
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
            await PopupNavigation.PushAsync(new AddDeleveryPopupPage(this));
        }

        public async void Load()
	    {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var deleveryMethodRestClient = new RestClient<DeleveryMethod>();
            try
            {
                List.ItemsSource = await deleveryMethodRestClient.GetAsync();
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
            await PopupNavigation.PushAsync(new EditDeleveryMethod(((e.Item) as DeleveryMethod), this));
        }

	    private async void RemoveDeleveryMethod(object sender, EventArgs e)
	    {
            var choix = await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer cette méthode de livraison?", "Oui", "Annuler");
            if (!choix) return;
            var id = (Int32.Parse((((sender as Image).Parent as StackLayout).Children[0] as Label).Text));
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var deleveryRC = new DeleveryMethodRestClient();
            var cat = await deleveryRC.GetAsyncById(id);
            if (cat.Orders.Any() ||
                cat.PastryShopDeleveryMethods.Any())
            {
                await PopupNavigation.PopAllAsync();
                await
                        DisplayAlert("Erreur",
                            "Impossible de supprimer cette méthode de paiement, une ou plusieurs pâtisseries et / ou commandes l'utilisent",
                            "Ok");
                return;
            }
            try
            {
                if (!(await deleveryRC.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la suppression de la méthode de livraison, veuillez réessayer plus tard.",
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
