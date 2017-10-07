using Kmandili.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopMasterDetailPage
    {
        public bool HasNavigatedToEdit = false;

		public PastryShopMasterDetailPage (PastryShop pastryShop)
		{
		    InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            Master = new PastryShopMasterPage(this, pastryShop);
		    var pastryShopProfile = new PPastryShopProfile(this, pastryShop);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Detail = new NavigationPage(pastryShopProfile);
                    break;
                case Device.Android:
                    Detail = new NavigationPage(pastryShopProfile);
                    break;
                case Device.UWP:
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
            if(IsPresented)
                (Master as PastryShopMasterPage)?.UpdateOrderNotificationNumber();
        }
	}
}
