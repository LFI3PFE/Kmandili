using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOrderDetail
	{
        private Order _order;
	    private readonly UserOrderList _userOrderList;
	    private bool _updateParent;

        public UserOrderDetail (UserOrderList userOrderList, Order order)
		{
			InitializeComponent ();
            ProductsList.SeparatorVisibility = SeparatorVisibility.None;
            if (order.Status.StatusName == "En Attente")
            {
                ToolbarItem canceToolbarItem = new ToolbarItem()
                {
                    Icon = "close.png",
                    Text = "Annuler"
                };
                canceToolbarItem.Clicked += CanceToolbarItem_Clicked;
                ToolbarItems.Add(canceToolbarItem);
            }else if (order.Status.StatusName == "Livrée")
            {
                ToolbarItem confirmToolbarItem = new ToolbarItem()
                {
                    Icon = "confirm.png",
                    Text = "Reçus"
                };
                confirmToolbarItem.Clicked += ConfirmToolbarItem_Clicked;
                ToolbarItems.Add(confirmToolbarItem);
            }
            _order = order;
		    _userOrderList = userOrderList;
            UpdateView();
		    Task.Run(() => MarkAsSeen());
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

	    protected override async void OnAppearing()
	    {
	        if (_order.Status.StatusName == "Livrée" && !_order.SeenUser)
	        {
	            await DisplayAlert("Remarque", "Votre commande a été marqué comme \"Livrée\", merci de la marquer comme \"Reçue\" dés que vous la recevrez!", "Ok");
	        }
	    }

	    private async void MarkAsSeen()
	    {
	        if (!_order.SeenUser)
	        {
                OrderRestClient orderRc = new OrderRestClient();
	            try
	            {
                    if (await orderRc.MarkAsSeenUser(_order.ID))
                        _updateParent = true;
                }
	            catch (Exception)
	            {
                    await PopupNavigation.PopAllAsync();
                    await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                    await Navigation.PopAsync();
	            }
	        }
	    }

	    protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
	        if (_updateParent)
	        {
	            _userOrderList.Load();
	        }
	    }

	    private async void ConfirmToolbarItem_Clicked(object sender, EventArgs e)
	    {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            Order newOrder = new Order()
            {
                ID = _order.ID,
                PastryShop_FK = _order.PastryShop_FK,
                User_FK = _order.User_FK,
                Status_FK = 5,
                DeleveryMethod_FK = _order.DeleveryMethod_FK,
                PaymentMethod_FK = _order.PaymentMethod_FK,
                Date = _order.Date,
                SeenUser = true,
                SeenPastryShop = false,
            };
            OrderRestClient orderRc = new OrderRestClient();
	        try
	        {
                if (await orderRc.PutAsync(newOrder.ID, newOrder))
                {
                    _order = await orderRc.GetAsyncById(_order.ID);
                    if (_order == null) return;
                    EmailRestClient emailRc = new EmailRestClient();
                    await emailRc.SendOrderEmail(_order.ID);
                    _updateParent = true;
                    ToolbarItems.Clear();
                    UpdateView();
                    await PopupNavigation.PopAsync();
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
                    _userOrderList.Load();
                    await DisplayAlert("Succès", "Votre commande a été annuler.", "Ok");
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
