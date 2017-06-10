using System.IO;
using Kmandili.Interfaces;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WebViewTest : ContentPage
	{
		public WebViewTest ()
		{
			InitializeComponent ();
		    ShowLoadingScreen();
            Browser.Navigated += Browser_Navigated;
            var urlWebView = new UrlWebViewSource
            {
                Url = App.ServerURL + "api/GetChartsView/" + 1,
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
