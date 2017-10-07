using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.Charts;
using Kmandili.Views.PastryShopViews.EditProfile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.OrderViewsAndFilter;

namespace Kmandili.Views.PastryShopViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopMasterPage
	{
	    private readonly PastryShopMasterDetailPage _pastryShopMasterDetailPage;
	    private PastryShop _pastryShop;
		public PastryShopMasterPage (PastryShopMasterDetailPage pastryShopMasterDetailPage, PastryShop pastryShop)
		{
			InitializeComponent ();
		    _pastryShopMasterDetailPage = pastryShopMasterDetailPage;
		    _pastryShop = pastryShop;
            UpdateOrderNotificationNumber(pastryShop.Orders.ToList());
        }

	    public void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

        public async void UpdateOrderNotificationNumber()
        {
            PastryShopRestClient pastryShopRestClient = new PastryShopRestClient();
            try
            {
                _pastryShop = await pastryShopRestClient.GetAsyncById(_pastryShop.ID);
                if (_pastryShop == null) return;
                int number = _pastryShop.Orders.Count(o => !o.SeenPastryShop);
                NotificationsNumber.Source = (number != 0 ? (number > 9 ? "_9plus.png" : "_" + number + ".png") : "");
            }
            catch (HttpRequestException)
            {
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
            }
        }

        public void UpdateOrderNotificationNumber(List<Order> orders)
        {
            int number = orders.Count(o => !o.SeenPastryShop);
            NotificationsNumber.Source = (number != 0 ? (number > 9 ? "_9plus.png" : "_" + number + ".png") : "");
        }

        private async void ToOrderList(object sender, EventArgs e)
        {
            _pastryShopMasterDetailPage.IsPresented = false;
            await _pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new PsOrderList(_pastryShop.ID));
        }


        private async void ToEditProfile(object sender, EventArgs e)
	    {
            _pastryShopMasterDetailPage.IsPresented = false;
            _pastryShopMasterDetailPage.HasNavigatedToEdit = true;
            await _pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new EditProfileInfo(_pastryShop.ID, true));
        }

	    private async void ToChart(object sender, EventArgs e)
	    {
            _pastryShopMasterDetailPage.IsPresented = false;
	        var doughnutPage = new Doughnut()
	        {
                Icon = "",
                Title = "Meilleurs produits"
	        };
	        var linePage = new Line(_pastryShop)
	        {
	            Icon = "",
	            Title = "Commandes"
	        };
	        var earningsPage = new PEarningsChart(_pastryShop)
	        {
	            Title = "Revenus"
	        };
            TabbedPage chartsPage = new TabbedPage()
            {
                Children =
                {
                    earningsPage,
                    doughnutPage,
                    linePage,
                },
            };
            await _pastryShopMasterDetailPage.Detail.Navigation.PushAsync(chartsPage);
        }
    }
}
