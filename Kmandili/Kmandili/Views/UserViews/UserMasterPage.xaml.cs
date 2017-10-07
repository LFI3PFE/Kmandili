using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Helpers;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews.OrderViewsAndFilter;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMasterPage
	{
	    private readonly UserMasterDetailPage _userMasterDetailPage;
	    private User _user;
		public UserMasterPage (UserMasterDetailPage userMasterDetailPage, User user)
		{
			InitializeComponent ();
		    _user = user;
		    _userMasterDetailPage = userMasterDetailPage;
		    UpdateOrderNotificationNumber(user.Orders.ToList());
		}

        private void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

        public async void UpdateOrderNotificationNumber()
        {
            UserRestClient userRc = new UserRestClient();
            try
            {
                _user = await userRc.GetAsyncById(Settings.Id);
            }
            catch (HttpRequestException)
            {
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                return;
            }
            if(_user == null) return;
            int number = _user.Orders.Count(o => !o.SeenUser);
            NotificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        public void UpdateOrderNotificationNumber(List<Order> orders)
	    {
            int number = orders.Count(o => !o.SeenUser);
	        NotificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        private async void ToOrderList(object sender, EventArgs e)
        {
            _userMasterDetailPage.IsPresented = false;
            if (_userMasterDetailPage.Detail.GetType().Name == "UserOrderList") return;
            await _userMasterDetailPage.Detail.Navigation.PushAsync(new UserOrderList());
        }

	    private async void UpdateUser_OnTapped(object sender, EventArgs e)
	    {
            _userMasterDetailPage.IsPresented = false;
            if (_userMasterDetailPage.Detail.GetType().Name == "EditUserProfile") return;
            await _userMasterDetailPage.Detail.Navigation.PushAsync(new EditUserProfile(Settings.Id));
	    }

	    private async void ToCart(object sender, EventArgs e)
	    {
            _userMasterDetailPage.IsPresented = false;
            if (_userMasterDetailPage.Detail.GetType().Name == "Cart") return;
            await _userMasterDetailPage.Detail.Navigation.PushAsync(new Cart());
        }
    }
}
