using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOrderDetail : ContentPage
	{
        private Order order;
	    private UserOrderList userOrderList;
	    private bool updateParent = false;

        public UserOrderDetail (UserOrderList userOrderList, Order order)
		{
			InitializeComponent ();
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
            this.order = order;
		    this.userOrderList = userOrderList;
            updateView();
		    Task.Run(() => MarkAsSeen());
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

	    protected async override void OnAppearing()
	    {
	        if (order.Status.StatusName == "Livrée" && !order.SeenUser)
	        {
	            await DisplayAlert("Remarque", "Votre commande a été marqué comme \"Livrée\", merci de la marquer comme \"Reçue\" dés que vous la recevrez!", "Ok");
	        }
	    }

	    private async void MarkAsSeen()
	    {
	        if (!order.SeenUser)
	        {
                OrderRestClient orderRC = new OrderRestClient();
	            try
	            {
                    if (await orderRC.MarkAsSeenUser(order.ID))
                        updateParent = true;
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
	        if (updateParent)
	        {
	            userOrderList.load();
	        }
	    }

	    private async void ConfirmToolbarItem_Clicked(object sender, EventArgs e)
	    {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            Order newOrder = new Order()
            {
                ID = order.ID,
                PastryShop_FK = order.PastryShop_FK,
                User_FK = order.User_FK,
                Status_FK = 5,
                DeleveryMethod_FK = order.DeleveryMethod_FK,
                PaymentMethod_FK = order.PaymentMethod_FK,
                Date = order.Date,
                SeenUser = true,
                SeenPastryShop = false,
            };
            OrderRestClient orderRC = new OrderRestClient();
	        try
	        {
                if (await orderRC.PutAsync(newOrder.ID, newOrder))
                {
                    order = await orderRC.GetAsyncById(order.ID);
                    if (order == null) return;
                    EmailRestClient emailRC = new EmailRestClient();
                    await emailRC.SendOrderEmail(order.ID);
                    updateParent = true;
                    ToolbarItems.Clear();
                    updateView();
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
                return;
            }
        }


        private async void CanceToolbarItem_Clicked(object sender, EventArgs e)
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
                if (!await emailRC.SendCancelOrderEmail(order.ID)) return;
                if (await orderRC.DeleteAsync(order.ID))
                {
                    userOrderList.load();
                    await DisplayAlert("Succès", "Votre commande a été annuler.", "Ok");
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
                return;
            }
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
