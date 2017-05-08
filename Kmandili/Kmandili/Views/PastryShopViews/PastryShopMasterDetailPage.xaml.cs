using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopMasterDetailPage : MasterDetailPage
	{
        private PastryShop pastryShop;
        private PastryShopProfile pastryShopProfile;

		public PastryShopMasterDetailPage (PastryShop pastryShop)
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            this.pastryShop = pastryShop;
            Master = new PastryShopMasterPage(this, pastryShop);
            //Detail = new PastryShopProfile(this, pastryShop);
		    pastryShopProfile = new PastryShopProfile(this, pastryShop);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Detail = new NavigationPage(pastryShopProfile);
                    break;
                case Device.Android:
                    Detail = new NavigationPage(pastryShopProfile);
                    break; ;
                case Device.WinPhone:
                case Device.Windows:
                    Detail = pastryShopProfile;
                    break;
                default:
                    Detail = new NavigationPage(pastryShopProfile);
                    break;
            }
        }

        protected async override void OnAppearing()
        {
            OrderRestClient orderRC = new OrderRestClient();
            var orders = await orderRC.GetAsyncByPastryShopID(App.Connected.Id);
            if (orders == null) return;
            (Master as PastryShopMasterPage)?.UpdateOrderNotificationNumber(orders);
        }

        public void ReloadPastryShop()
        {
            pastryShopProfile.Reload();
        }
	}
}
