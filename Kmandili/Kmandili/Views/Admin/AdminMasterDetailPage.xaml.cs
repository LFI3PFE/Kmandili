using Kmandili.Views.Admin.PSViews.PastryShopListAndFilter;
using Kmandili.Views.Admin.UserViews.UserListAndFilter;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdminMasterDetailPage : MasterDetailPage
    {
        private class MyTabbedPage : TabbedPage
        {
            protected override bool OnBackButtonPressed()
            {
                return true;
            }
        }

        public AdminMasterDetailPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
#pragma warning disable CS0618 // Type or member is obsolete
            MyTabbedPage tab = new MyTabbedPage()
            {
                Children =
                {
                    new UsersList()
                    {
                        Icon = Device.OnPlatform("clients.png", "", ""),
                        Title = "Clients"
                    },
                    new APastryShopList()
                    {
                        Icon = Device.OnPlatform("shop.png", "", ""),
                        Title = "Pâtisseries"
                    }
                }
            };
#pragma warning restore CS0618 // Type or member is obsolete
            Master = new AdminMasterPage(this);
            //Detail = new NavigationPage(tab);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.Android:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.UWP:
                    Detail = tab;
                    break;
                default:
                    Detail = new NavigationPage(tab); ;
                    break;
            }
            //IsPresentedChanged += UserMasterDetailPage_IsPresentedChanged;
        }

	    protected override bool OnBackButtonPressed()
	    {
	        return true;
	    }

	    //private void UserMasterDetailPage_IsPresentedChanged(object sender, EventArgs e)
        //{
        //    if (IsPresented)
        //        (Master as AdminMasterPage).UpdateOrderNotificationNumber();
        //}
    }
}
