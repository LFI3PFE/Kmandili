using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews.OrderViewsAndFilter;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterPage : ContentPage
	{
	    private UserMasterDetailPage userMasterDetailPage;
	    private User user;
		public UserMasterPage (UserMasterDetailPage userMasterDetailPage, User user)
		{
			InitializeComponent ();
		    this.user = user;
		    this.userMasterDetailPage = userMasterDetailPage;
		    UpdateOrderNotificationNumber(user.Orders.ToList());
		}

        private void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

        public async void UpdateOrderNotificationNumber()
        {
            UserRestClient userRC = new UserRestClient();
            user = await userRC.GetAsyncById(Settings.Id);
            if(user == null) return;
            int number = user.Orders.Count(o => !o.SeenUser);
            NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        public void UpdateOrderNotificationNumber(List<Order> orders)
	    {
            int number = orders.Count(o => !o.SeenUser);
	        NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        private async void ToOrderList(object sender, EventArgs e)
        {
            userMasterDetailPage.IsPresented = false;
            if (userMasterDetailPage.Detail.GetType().Name == "UserOrderList") return;
            await userMasterDetailPage.Detail.Navigation.PushAsync(new UserOrderList());
        }

	    private async void UpdateUser_OnTapped(object sender, EventArgs e)
	    {
            userMasterDetailPage.IsPresented = false;
            if (userMasterDetailPage.Detail.GetType().Name == "EditUserProfile") return;
            await userMasterDetailPage.Detail.Navigation.PushAsync(new EditUserProfile());
	    }

	    private async void ToCart(object sender, EventArgs e)
	    {
            userMasterDetailPage.IsPresented = false;
            if (userMasterDetailPage.Detail.GetType().Name == "Cart") return;
            await userMasterDetailPage.Detail.Navigation.PushAsync(new Cart());
        }
	}
}
