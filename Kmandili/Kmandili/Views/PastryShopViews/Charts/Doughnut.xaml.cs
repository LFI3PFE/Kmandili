using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.Charts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Doughnut : ContentPage
	{
        public Doughnut()
        {
            InitializeComponent();
            Browser.Navigated += Browser_Navigated;
            
        }

	    protected override void OnAppearing()
        {
            ShowLoadingScreen();
            var urlWebView = new UrlWebViewSource
            {
                Url = App.ServerURL + "api/GetDoughnutChartView/" + Settings.Id,
            };
            Browser.Source = urlWebView;
        }

	    private async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            await PopupNavigation.PopAllAsync();
        }

        private async void ShowLoadingScreen()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
        }
    }
}
