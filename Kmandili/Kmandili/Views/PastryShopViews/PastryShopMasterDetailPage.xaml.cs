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
        private PPastryShopProfile pastryShopProfile;
        public bool hasNavigatedToEdit = false;

		public PastryShopMasterDetailPage (PastryShop pastryShop)
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            Master = new PastryShopMasterPage(this, pastryShop);
		    pastryShopProfile = new PPastryShopProfile(this, pastryShop);
            //Detail = new NavigationPage(pastryShopProfile);
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
            IsPresentedChanged += PastryShopMasterDetailPage_IsPresentedChanged;
        }

        private void PastryShopMasterDetailPage_IsPresentedChanged(object sender, System.EventArgs e)
        {
            var x = Master;
            if(IsPresented)
                (Master as PastryShopMasterPage).UpdateOrderNotificationNumber();
        }
	}
}
