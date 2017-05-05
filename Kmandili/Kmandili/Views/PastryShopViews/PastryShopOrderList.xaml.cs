using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopOrderList : ContentPage
	{
		public PastryShopOrderList ()
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
            OrderList.ItemsSource = orders = await orderRC.GetAsyncByPastryShopID(App.Connected.Id);
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
	        await Navigation.PushAsync(new PastryShopOrderDetail(this, e.Item as Order));
	    }

    }
}
