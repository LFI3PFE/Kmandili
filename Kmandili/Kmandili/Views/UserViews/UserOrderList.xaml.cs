using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOrderList : ContentPage
	{

		public UserOrderList ()
		{
			InitializeComponent ();
            load();
		}

        public async void load()
        {
            OrderList.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            OrderRestClient orderRC = new OrderRestClient();
            List<Order> orders;
            OrderList.ItemsSource = orders = await orderRC.GetAsyncByUserID(App.Connected.Id);
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            OrderList.IsVisible = true;
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private async void ToOrderDetail(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new UserOrderDetail(this, e.Item as Order));
        }
	}
}
