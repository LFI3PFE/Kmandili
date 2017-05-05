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
		public PastryShopMasterDetailPage (PastryShop pastryShop)
		{
			InitializeComponent ();
            this.pastryShop = pastryShop;
            Master = new PastryShopMasterPage(this, pastryShop);
            Detail = new NavigationPage(new PastryShopProfile(this, pastryShop));
		}

        protected async override void OnAppearing()
        {
            OrderRestClient orderRC = new OrderRestClient();
            (Master as PastryShopMasterPage).updateOrderNotificationNumber(await orderRC.GetAsyncByPastryShopID(App.Connected.Id));
        }
	}
}
