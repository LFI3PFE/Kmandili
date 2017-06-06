using System.IO;
using Kmandili.Interfaces;
using Kmandili.Models.RestClient;
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
            var urlWebView = new UrlWebViewSource
            {
                Url = App.ServerURL + "api/GetChartsView/" + 1,
            };
            Browser.Source = urlWebView;
		}
	}
}
