using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.EditProfile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.OrderViewsAndFilter;

namespace Kmandili.Views.PastryShopViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopMasterPage : ContentPage
	{
	    private PastryShopMasterDetailPage pastryShopMasterDetailPage;
	    private PastryShop pastryShop;
		public PastryShopMasterPage (PastryShopMasterDetailPage pastryShopMasterDetailPage, PastryShop pastryShop)
		{
			InitializeComponent ();
		    this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
		    this.pastryShop = pastryShop;
            UpdateOrderNotificationNumber(pastryShop.Orders.ToList());
        }

	    protected async override void OnAppearing()
	    {
	        PastryShopRestClient pastryShopRC = new PastryShopRestClient();
	        pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
	    }

	    public void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

        public async void UpdateOrderNotificationNumber()
        {
            PastryShopRestClient pastryShopRestClient = new PastryShopRestClient();
            pastryShop = await pastryShopRestClient.GetAsyncById(pastryShop.ID);
            if (pastryShop == null) return;
            int number = pastryShop.Orders.Count(o => !o.SeenPastryShop);
            NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        public void UpdateOrderNotificationNumber(List<Order> orders)
        {
            int number = orders.Count(o => !o.SeenPastryShop);
            NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        private async void ToOrderList(object sender, EventArgs e)
        {
            pastryShopMasterDetailPage.IsPresented = false;
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new PSOrderList());
            //NavigationPage nav = new NavigationPage(new ContentPage());
            //await nav.PushAsync(new PastryShopOrderList());
            //await Navigation.PushModalAsync(nav);
        }

	    private async void ToEditDeleveryMethod(object sender, EventArgs e)
	    {
            pastryShopMasterDetailPage.IsPresented = false;
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new EditDeleveryMethods(pastryShopMasterDetailPage));
        }


        private async void ToEditProfile(object sender, EventArgs e)
	    {
            pastryShopMasterDetailPage.IsPresented = false;
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new EditProfileInfo(pastryShopMasterDetailPage));
        }
        
    }
}
