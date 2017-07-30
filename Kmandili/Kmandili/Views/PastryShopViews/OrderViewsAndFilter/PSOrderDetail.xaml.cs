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

namespace Kmandili.Views.PastryShopViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PSOrderDetail : ContentPage
	{
	    private PSOrderList pastryShopOrderList;
	    private Order order;
	    private bool updateParent = false;
	    private List<Status> status;
	    private ToolbarItem confirmToolbarItem;
		public PSOrderDetail(PSOrderList pastryShopOrderList, Order order)
		{
            InitializeComponent ();
            load();
		    confirmToolbarItem = new ToolbarItem()
		    {
		        Icon = "confirm.png",
		        Text = "Confirmer",
		        Order = ToolbarItemOrder.Primary
		    };
            confirmToolbarItem.Clicked += ConfirmToolbarItem_Clicked;
            this.order = order;
		    this.pastryShopOrderList = pastryShopOrderList;
		    ProductsList.ItemsSource = order.OrderProducts;
		    ProductListViewLayout.HeightRequest = order.OrderProducts.Count*100;
            OrderID.Text = order.ID.ToString();
		    ClientName.Text = order.User.Name + " " + order.User.LastName;
		    Date.Text = order.Date.ToString("d");
		    Delevery.Text = order.DeleveryMethod.DeleveryType;
		    Payment.Text = order.Payment.PaymentMethod;
            Total.Text = order.OrderProducts.Sum(op => op.Quantity * op.Product.Price).ToString();
            Status.SelectedIndexChanged += Status_SelectedIndexChanged;
		    Task.Run(() => MarkAsSeen());
		}

        private async void ConfirmToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            Order newOrder = new Order()
            {
                ID = order.ID,
                PastryShop_FK = order.PastryShop_FK,
                User_FK = order.User_FK,
                Status_FK = (Status.SelectedItem as Status).ID,
                DeleveryMethod_FK = order.DeleveryMethod_FK,
                PaymentMethod_FK = order.PaymentMethod_FK,
                Date = order.Date,
                SeenUser = false,
                SeenPastryShop = true,
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
                    load();
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

        private void Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolbarItems.Clear();
            if ((sender as Picker).SelectedItem != null && ((sender as Picker).SelectedItem as Status).StatusName != order.Status.StatusName)
            {
                ToolbarItems.Add(confirmToolbarItem);
            }
        }

        private async void load()
	    {
            if (status == null || status.Count == 0)
            {
                RestClient<Status> statusRC = new RestClient<Status>();
                try
                {
                    status = await statusRC.GetAsync();
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
                if(status == null) return;
            }
            if (order.Status.StatusName == "En Attente")
            {
                Status.ItemsSource = status.GetRange(0, 3);
                
            }else if (order.Status.StatusName == "Acceptée")
            {
                Status.ItemsSource = status.GetRange(1, 1).Concat(status.GetRange(3 ,1)).ToList();
            }
            else if(order.Status.StatusName == "Livrée")
            {
                Status.ItemsSource = status.GetRange(3, 1);
            }
            else
            {
                Status.ItemsSource = status.GetRange(status.IndexOf(status.FirstOrDefault(s => s.StatusName == order.Status.StatusName)), 1);
            }
            Status.SelectedIndex = Status.ItemsSource.IndexOf(status.FirstOrDefault(s => s.StatusName == order.Status.StatusName));
        }

        private async void MarkAsSeen()
        {
            if (!order.SeenPastryShop)
            {
                OrderRestClient orderRC = new OrderRestClient();
                try
                {
                    if (await orderRC.MarkAsSeenPastryShop(order.ID))
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
                pastryShopOrderList.load();
            }
        }

        private void SelectedNot(object sender, EventArgs e)
	    {
	        (sender as ListView).SelectedItem = null;
	    }

    }
}
