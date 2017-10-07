using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.PSViews.Charts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AEarningsChart : ContentPage
	{
        private ToolbarItem refreshToolbarItem;
        private int year;
        private int semester;
        private DateTime max, min;
        private PastryShop pastryShop;

        public AEarningsChart(PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            refreshToolbarItem = new ToolbarItem()
            {
                Text = "Rafraîchir",
                Order = ToolbarItemOrder.Primary,
                Icon = "refresh.png"
            };
            refreshToolbarItem.Clicked += RefreshToolbarItem_Clicked;
            ToolbarItems.Add(refreshToolbarItem);
            min = pastryShop.JoinDate;
            max = DateTime.Now;
            year = max.Year;
            semester = getSemester(max.Month);
            Load();
        }

        private void PrecedentTapped(object sender, EventArgs e)
        {
            if (min.Year == year && getSemester(min.Month) == semester) return;
            if (semester == 2)
                semester--;
            else
            {
                semester = 2;
                year--;
            }
            Load();
        }

        private void SuivantTapped(object sender, EventArgs e)
        {
            if (max.Year == year && getSemester(max.Month) == semester) return;
            if (semester == 1)
                semester++;
            else
            {
                semester = 1;
                year++;
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
            var chartRC = new ChartsRestClient();
            try
            {
                var htmlWebView = new HtmlWebViewSource()
                {
                    Html = await chartRC.GetChartView(App.ServerUrl + "api/GetIncomsChartView/" + pastryShop.ID + "/" + year + "/" + semester)
                };
                Browser.Source = htmlWebView;
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
            if (max.Year == year && getSemester(max.Month) == semester)
            {
                SuivantLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                SuivantLabel.TextColor = Color.DodgerBlue;
            }

            if (min.Year == year && getSemester(min.Month) == semester)
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

        private int getSemester(int month)
        {
            return month <= 6 ? 1 : 2;
        }
    }
}
