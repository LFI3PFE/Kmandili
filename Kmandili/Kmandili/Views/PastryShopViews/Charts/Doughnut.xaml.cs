using System;
using System.Net.Http;
using Kmandili.Helpers;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.Charts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Doughnut
	{
	    public Doughnut()
        {
            InitializeComponent();
            var refreshToolbarItem = new ToolbarItem()
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
            BodyLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var chartRc = new ChartsRestClient();
            try
            {
                var htmlWebSource = new HtmlWebViewSource()
                {
                    Html = await chartRc.GetChartView(App.ServerUrl + "api/GetDoughnutChartView/" + Settings.Id)
                };
                Browser.Source = htmlWebSource;
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            BodyLayout.IsVisible = true;
        }
    }
}
