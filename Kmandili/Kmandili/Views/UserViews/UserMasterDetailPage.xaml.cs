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
            pastryShopList = new PastryShopList()
            {
                Title = "Liste des pâtisseries"
            };
            productList = new ProductList()
            {
                Title = "Liste des Produits"
            };
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
