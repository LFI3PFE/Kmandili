using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterDetailPage : MasterDetailPage
	{
        private class MyTabbedPage : TabbedPage
        {
            private UserMasterDetailPage userMasterDetailPage;
            public MyTabbedPage(UserMasterDetailPage userMasterDetailPage)
            {
                this.userMasterDetailPage = userMasterDetailPage;
            }
            protected async override void OnAppearing()
            {
                OrderRestClient orderRC = new OrderRestClient();
                (userMasterDetailPage.Master as UserMasterPage).updateOrderNotificationNumber(await orderRC.GetAsyncByUserID(App.Connected.Id));
            }
        }

        private PastryShopList pastryShopList;
        private ProductList productList;
        
        public UserMasterDetailPage (User user)
		{
			InitializeComponent ();
            pastryShopList = new PastryShopList()
            {
                #pragma warning disable CS0618 // Type or member is obsolete
                Icon = Device.OnPlatform("shop.png", null, "shop.png"),
                #pragma warning restore CS0618 // Type or member is obsolete
                Title = "Liste des pâtisseries"
            };
            productList = new ProductList()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Icon = Device.OnPlatform("products.png", null, "products.png"),
#pragma warning restore CS0618 // Type or member is obsolete
                Title = "Liste des Produits"
            };
            MyTabbedPage tab = new MyTabbedPage(this)
            {
                Children =
                {
                    pastryShopList,
                    productList
                }
            };
            Master = new UserMasterPage(this, user);
            Detail = new NavigationPage(tab);
		}

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
	}
}
