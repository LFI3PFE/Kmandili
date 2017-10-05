using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.UserViews.Orders
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AUOrderDetail : ContentPage
	{
        private Order order;
	    private AUOrderList orderList;
        private bool updateParent = false;

        public AUOrderDetail(AUOrderList orderList, Order order)
        {
            InitializeComponent();
            if (order.Status.StatusName != "Reçue" && order.Status.StatusName != "Livrée" && order.Status.StatusName != "Refusée")
            {
                ToolbarItem canceToolbarItem = new ToolbarItem()
                {
                    Icon = "close.png",
                    Text = "Annuler"
                };
                canceToolbarItem.Clicked += CanceToolbarItem_Clicked;
                ToolbarItems.Add(canceToolbarItem);
            }
            this.order = order;
            this.orderList = orderList;
            updateView();
        }

        private void updateView()
        {
            ProductsList.ItemsSource = order.OrderProducts;
            ProductListViewLayout.HeightRequest = order.OrderProducts.Count * 100;
            OrderID.Text = order.ID.ToString();
            PastryShopName.Text = order.PastryShop.Name;
            Date.Text = order.Date.ToString("d");
            Delevery.Text = order.DeleveryMethod.DeleveryType;
            Payment.Text = order.Payment.PaymentMethod;
            Status.Text = order.Status.StatusName;
            Total.Text = order.OrderProducts.Sum(op => op.Quantity * op.Product.Price).ToString();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (updateParent)
            {
                orderList.load();
            }
        }

        private async void CanceToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var choix = await DisplayAlert("Confirmation",
                "Etes-vous sur de vouloir annuler cette commande?",
                "Confirmer", "Annuler");
            if (!choix) return;
            OrderRestClient orderRC = new OrderRestClient();
            EmailRestClient emailRC = new EmailRestClient();
            try
            {
                if (!await emailRC.SendCancelOrderEmail(order.ID)) return;
                if (await orderRC.DeleteAsync(order.ID))
                {
                    orderList.load();
                    await DisplayAlert("Succès", "Commande annulée avec succès.", "Ok");
                    App.updateClientList = true;
                    await PopupNavigation.PopAllAsync();
                    await Navigation.PopAsync();
                }
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
                return;
            }
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
