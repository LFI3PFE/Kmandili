using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.Charts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PEarningsChart
	{
	    private int _year;
        private int _semester;
        private DateTime _max, _min;
        private readonly PastryShop _pastryShop;

        public PEarningsChart(PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            var refreshToolbarItem = new ToolbarItem()
            {
                Text = "Rafraîchir",
                Order = ToolbarItemOrder.Primary,
                Icon = "refresh.png"
            };
            refreshToolbarItem.Clicked += RefreshToolbarItem_Clicked;
            ToolbarItems.Add(refreshToolbarItem);
            _min = pastryShop.JoinDate;
            _max = DateTime.Now;
            _year = _max.Year;
            _semester = GetSemester(_max.Month);
            Load();
        }

        private void PrecedentTapped(object sender, EventArgs e)
        {
            if (_min.Year == _year && GetSemester(_min.Month) == _semester) return;
            if (_semester == 2)
                _semester--;
            else
            {
                _semester = 2;
                _year--;
            }
            Load();
        }

        private void SuivantTapped(object sender, EventArgs e)
        {
            if (_max.Year == _year && GetSemester(_max.Month) == _semester) return;
            if (_semester == 1)
                _semester++;
            else
            {
                _semester = 1;
                _year++;
            }
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
                var htmlWebView = new HtmlWebViewSource()
                {
                    Html = await chartRc.GetChartView(App.ServerUrl + "api/GetIncomsChartView/" + _pastryShop.ID + "/" + _year + "/" + _semester)
                };
                Browser.Source = htmlWebView;
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
            if (_max.Year == _year && GetSemester(_max.Month) == _semester)
            {
                SuivantLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                SuivantLabel.TextColor = Color.DodgerBlue;
            }

            if (_min.Year == _year && GetSemester(_min.Month) == _semester)
            {
                PrecedentLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                PrecedentLabel.TextColor = Color.DodgerBlue;
            }
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            BodyLayout.IsVisible = true;
        }

        private int GetSemester(int month)
        {
            return month <= 6 ? 1 : 2;
        }
    }
}
