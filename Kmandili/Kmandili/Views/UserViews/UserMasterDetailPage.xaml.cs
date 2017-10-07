using System;
using Kmandili.Models;
using Kmandili.Views.UserViews.PastryShopListAndFilter;
using Kmandili.Views.UserViews.ProductListAndFilter;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterDetailPage
	{
        private class MyTabbedPage : TabbedPage
        {
            protected override bool OnBackButtonPressed()
            {
                return true;
            }
        }

	    public UserMasterDetailPage (User user)
		{
		    InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

#pragma warning disable CS0618 // Type or member is obsolete
            var pastryShopList = new UPastryShopList()
            {
                Icon = Device.OnPlatform("shop.png", "", ""),

                Title = "Liste des pâtisseries"
            };
            var productList = new ProductList()
            {
                Icon = Device.OnPlatform("products.png", "", ""),
                Title = "Liste des Produits"
            };
#pragma warning restore CS0618 // Type or member is obsolete
            MyTabbedPage tab = new MyTabbedPage()
            {
                Children =
                {
                    pastryShopList,
                    productList
                }
            };
            Master = new UserMasterPage(this, user);
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
                    Detail = new NavigationPage(tab);
                    break;
            }
            IsPresentedChanged += UserMasterDetailPage_IsPresentedChanged;
        }

        private void UserMasterDetailPage_IsPresentedChanged(object sender, EventArgs e)
        {
            if (IsPresented)
                (Master as UserMasterPage)?.UpdateOrderNotificationNumber();
        }
    }
}
