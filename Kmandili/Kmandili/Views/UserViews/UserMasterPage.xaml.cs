using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterPage : ContentPage
	{
	    private UserMasterDetailPage userMasterDetailPage;
		public UserMasterPage (UserMasterDetailPage userMasterDetailPage, User user)
		{
			InitializeComponent ();
		    this.userMasterDetailPage = userMasterDetailPage;
		    var x = user.Orders.Count(o => !o.SeenUser);
		    if (x != 0)
		    {
		        NotificationNumber.Text = x.ToString();
		    }
		}

        private void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

	    public void updateOrderNotificationNumber(List<Order> orders)
	    {
            NotificationNumber.Text = orders.Count(o => !o.SeenUser).ToString();
	    }

        private async void ToOrderList(object sender, EventArgs e)
        {
            userMasterDetailPage.IsPresented = false;
            await userMasterDetailPage.Detail.Navigation.PushAsync(new UserOrderList());
        }

	    private async void UpdateUser_OnTapped(object sender, EventArgs e)
	    {
            userMasterDetailPage.IsPresented = false;
	        await userMasterDetailPage.Detail.Navigation.PushAsync(new EditUserProfile());
	    }

	    private async void ToCart(object sender, EventArgs e)
	    {
            userMasterDetailPage.IsPresented = false;
            await userMasterDetailPage.Detail.Navigation.PushAsync(new Cart());
	    }
	}
}
