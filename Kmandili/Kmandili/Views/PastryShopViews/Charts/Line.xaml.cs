using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.Charts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Line : ContentPage
	{
	    private ToolbarItem refreshToolbarItem;

		public Line ()
		{
			InitializeComponent ();
            refreshToolbarItem = new ToolbarItem()
            {
                Text = "Rafraîchir",
                Order = ToolbarItemOrder.Primary,
                Icon = "refresh.png"
            };
            refreshToolbarItem.Clicked += RefreshToolbarItem_Clicked;
            ToolbarItems.Add(refreshToolbarItem);
            Load();
        }

        private void RefreshToolbarItem_Clicked(object sender, EventArgs e)
        {
            Load();
        }

        private async void Load()
        {
            BrowserLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var chartRC = new ChartsRestClient();
            var htmlWebView = new HtmlWebViewSource()
            {
                Html = await chartRC.GetChartView(App.ServerURL + "api/GetLineChartView/" + Settings.Id)
            };
            Browser.Source = htmlWebView;
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            BrowserLayout.IsVisible = true;
        }
    }
}
