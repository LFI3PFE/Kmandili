using System;
using System.Globalization;
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
	public partial class AuOrderDetail
	{
        private readonly Order _order;
	    private readonly AuOrderList _orderList;
        private readonly bool _updateParent = false;

        public AuOrderDetail(AuOrderList orderList, Order order)
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
            _order = order;
            _orderList = orderList;
            UpdateView();
        }

        private void UpdateView()
        {
            ProductsList.ItemsSource = _order.OrderProducts;
            ProductListViewLayout.HeightRequest = _order.OrderProducts.Count * 100;
            OrderID.Text = _order.ID.ToString();
            PastryShopName.Text = _order.PastryShop.Name;
            Date.Text = _order.Date.ToString("d");
            Delevery.Text = _order.DeleveryMethod.DeleveryType;
            Payment.Text = _order.Payment.PaymentMethod;
            Status.Text = _order.Status.StatusName;
            Total.Text = _order.OrderProducts.Sum(op => op.Quantity * op.Product.Price).ToString(CultureInfo.InvariantCulture);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_updateParent)
            {
                _orderList.Load();
            }
        }

        private async void CanceToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var choix = await DisplayAlert("Confirmation",
                "Etes-vous sur de vouloir annuler cette commande?",
                "Confirmer", "Annuler");
            if (!choix) return;
            OrderRestClient orderRc = new OrderRestClient();
            EmailRestClient emailRc = new EmailRestClient();
            try
            {
                if (!await emailRc.SendCancelOrderEmail(_order.ID)) return;
                if (await orderRc.DeleteAsync(_order.ID))
                {
                    _orderList.Load();
                    await DisplayAlert("Succès", "Commande annulée avec succès.", "Ok");
                    App.UpdateClientList = true;
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
            }
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}
