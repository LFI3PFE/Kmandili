using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews.PastryShopListAndFilter;
using Kmandili.Views.UserViews.ProductListAndFilter;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterDetailPage : MasterDetailPage
	{
        private class MyTabbedPage : TabbedPage
        {
            protected override bool OnBackButtonPressed()
            {
                return true;
            }
        }

        private UPastryShopList pastryShopList;
        private ProductList productList;
        
        public UserMasterDetailPage (User user)
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

#pragma warning disable CS0618 // Type or member is obsolete
            pastryShopList = new UPastryShopList()
            {
                Icon = Device.OnPlatform("shop.png", "", ""),

                Title = "Liste des pâtisseries"
            };
            productList = new ProductList()
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
            //Detail = new NavigationPage(tab);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.Android:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.WinPhone:
                case Device.Windows:
                    Detail = tab;
                    break;
                default:
                    Detail = new NavigationPage(tab); ;
                    break;
            }
            IsPresentedChanged += UserMasterDetailPage_IsPresentedChanged;
        }

        private void UserMasterDetailPage_IsPresentedChanged(object sender, EventArgs e)
        {
            if(IsPresented)
                (Master as UserMasterPage).UpdateOrderNotificationNumber();
        }
    }
}
