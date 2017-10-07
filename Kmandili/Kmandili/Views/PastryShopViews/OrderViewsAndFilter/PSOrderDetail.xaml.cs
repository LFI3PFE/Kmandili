using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PsOrderDetail
	{
	    private readonly PsOrderList _pastryShopOrderList;
	    private Order _order;
	    private bool _updateParent;
	    private List<Status> _status;
	    private readonly ToolbarItem _confirmToolbarItem;
		public PsOrderDetail(PsOrderList pastryShopOrderList, Order order)
		{
            InitializeComponent ();
            ProductsList.SeparatorVisibility = SeparatorVisibility.None;
            Load();
		    _confirmToolbarItem = new ToolbarItem()
		    {
		        Icon = "confirm.png",
		        Text = "Confirmer",
		        Order = ToolbarItemOrder.Primary
		    };
            _confirmToolbarItem.Clicked += ConfirmToolbarItem_Clicked;
            _order = order;
		    _pastryShopOrderList = pastryShopOrderList;
		    ProductsList.ItemsSource = order.OrderProducts;
		    ProductListViewLayout.HeightRequest = order.OrderProducts.Count*100;
            OrderId.Text = order.ID.ToString();
		    ClientName.Text = order.User.Name + " " + order.User.LastName;
		    Date.Text = order.Date.ToString("d");
		    Delevery.Text = order.DeleveryMethod.DeleveryType;
		    Payment.Text = order.Payment.PaymentMethod;
            Total.Text = order.OrderProducts.Sum(op => op.Quantity * op.Product.Price).ToString(CultureInfo.InvariantCulture);
            Status.SelectedIndexChanged += Status_SelectedIndexChanged;
		    Task.Run(() => MarkAsSeen());
		}

        private async void ConfirmToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            Order newOrder = new Order()
            {
                ID = _order.ID,
                PastryShop_FK = _order.PastryShop_FK,
                User_FK = _order.User_FK,
                Status_FK = ((Status) Status.SelectedItem).ID,
                DeleveryMethod_FK = _order.DeleveryMethod_FK,
                PaymentMethod_FK = _order.PaymentMethod_FK,
                Date = _order.Date,
                SeenUser = false,
                SeenPastryShop = true,
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
                    Load();
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

        private void Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolbarItems.Clear();
            if (((Picker) sender).SelectedItem != null && (((Picker) sender)?.SelectedItem as Status)?.StatusName != _order.Status.StatusName)
            {
                ToolbarItems.Add(_confirmToolbarItem);
            }
        }

        private async void Load()
	    {
            if (_status == null || _status.Count == 0)
            {
                RestClient<Status> statusRc = new RestClient<Status>();
                try
                {
                    _status = await statusRc.GetAsync();
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
                if(_status == null) return;
            }
            if (_order.Status.StatusName == "En Attente")
            {
                Status.ItemsSource = _status.GetRange(0, 3);
                
            }else if (_order.Status.StatusName == "Acceptée")
            {
                Status.ItemsSource = _status.GetRange(1, 1).Concat(_status.GetRange(3 ,1)).ToList();
            }
            else if(_order.Status.StatusName == "Livrée")
            {
                Status.ItemsSource = _status.GetRange(3, 1);
            }
            else
            {
                Status.ItemsSource = _status.GetRange(_status.IndexOf(_status.FirstOrDefault(s => s.StatusName == _order.Status.StatusName)), 1);
            }
            Status.SelectedIndex = Status.ItemsSource.IndexOf(_status.FirstOrDefault(s => s.StatusName == _order.Status.StatusName));
        }

        private async void MarkAsSeen()
        {
            if (!_order.SeenPastryShop)
            {
                OrderRestClient orderRc = new OrderRestClient();
                try
                {
                    if (await orderRc.MarkAsSeenPastryShop(_order.ID))
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
                _pastryShopOrderList.Load(_order.PastryShop_FK);
            }
        }

        private void SelectedNot(object sender, EventArgs e)
	    {
	        ((ListView) sender).SelectedItem = null;
	    }

    }
}
