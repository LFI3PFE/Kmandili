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

        private PastryShopList pastryShopList;
        private ProductList productList;
        
        public UserMasterDetailPage (User user)
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);
#pragma warning disable 618
            pastryShopList = new PastryShopList()
            {
                Icon = Device.OnPlatform("shop.png", null, "shop.png"),
                Title = "Liste des pâtisseries"
            };
            productList = new ProductList()
            {
                Icon = Device.OnPlatform("products.png", null, "products.png"),
                Title = "Liste des Produits"
            };
#pragma warning restore 618
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
