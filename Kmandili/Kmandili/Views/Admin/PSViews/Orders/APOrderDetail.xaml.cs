using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.PSViews.Orders
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class APOrderDetail : ContentPage
	{
        private APOrderList orderList;
        private Order order;
        private bool updateParent = false;
        private List<Status> status;
        private ToolbarItem cancelToolbarItem;
        public APOrderDetail(APOrderList orderList, Order order)
        {
            InitializeComponent();
            if (order.Status.StatusName != "Reçue" && order.Status.StatusName != "Livrée" && order.Status.StatusName != "Refusée")
            {
                cancelToolbarItem = new ToolbarItem()
                {
                    Icon = "close.png",
                    Text = "Annuler",
                    Order = ToolbarItemOrder.Primary
                };
                cancelToolbarItem.Clicked += CancelToolbarItem_Clicked;
                ToolbarItems.Add(cancelToolbarItem);
            }
            this.order = order;
            this.orderList = orderList;
            ProductsList.ItemsSource = order.OrderProducts;
            ProductListViewLayout.HeightRequest = order.OrderProducts.Count * 100;
            OrderID.Text = order.ID.ToString();
            ClientName.Text = App.ToTitleCase(order.User.Name) + " " + App.ToTitleCase(order.User.LastName);
            Date.Text = order.Date.ToString("d");
            Delevery.Text = order.DeleveryMethod.DeleveryType;
            Payment.Text = order.Payment.PaymentMethod;
            Status.Text = order.Status.StatusName;
            Total.Text = order.OrderProducts.Sum(op => op.Quantity * op.Product.Price).ToString();
        }

        private async void CancelToolbarItem_Clicked(object sender, EventArgs e)
        {
            var choix = await DisplayAlert("Confirmation",
                "Etes-vous sur de vouloir annuler cette commande?",
                "Confirmer", "Annuler");
            if (!choix) return;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            OrderRestClient orderRC = new OrderRestClient();
            EmailRestClient emailRC = new EmailRestClient();
            try
            {
                if (!await emailRC.SendCanelOrderEmailByAdmin(order.ID)) return;
                if (await orderRC.DeleteAsync(order.ID))
                {
                    orderList.load(order.PastryShop_FK);
                    await DisplayAlert("Succès", "Commande annuler.", "Ok");
                    await PopupNavigation.PopAsync();
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
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (updateParent)
            {
                orderList.load(order.PastryShop_FK);
            }
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
