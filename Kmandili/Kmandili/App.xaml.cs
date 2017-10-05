using Kmandili.Models.LocalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Kmandili
{
    public partial class App : Application
    {
        public static string ServerURL = "http://kmandiliwebservice.servehttp.com:300/";
        public static List<CartPastry> Cart = new List<CartPastry>();
        public static bool galleryIsOpent = false;
        public static bool updatePastryList = false;
        public static bool updateClientList = false;
        public static bool isConnected = false;
        public App()
        {
            InitializeComponent();

            //MainPage = new WebViewTest();
            MainPage = new NavigationPage(new MainPage());
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        public static string ToTitleCase(string s)
        {
            return s.Substring(0, 1).ToUpper() + s.Substring(1, s.Length - 1);
        }

        public static bool TokenExpired()
        {
            if (string.IsNullOrEmpty(Settings.ExpireDate)) return false;
            var expireDate = DateTime.ParseExact(Settings.ExpireDate,
                    System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern,
                    System.Globalization.CultureInfo.CurrentCulture);
            var b = expireDate < DateTime.Now;
            if (b)
            {
                App.Current.MainPage.DisplayAlert("Erreur", "Session Experiée", "Ok");
                Logout();
            }
            return b;
        }

        public static void setMainPage(MasterDetailPage newMainPage)
        {
            //App.Current.MainPage = new NavigationPage(newMainPage);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    App.Current.MainPage = newMainPage;
                    break;
                case Device.Android:
                    App.Current.MainPage = new NavigationPage(newMainPage);
                    break;
                case Device.UWP:
                    App.Current.MainPage = new NavigationPage(newMainPage);
                    break;
                default:
                    App.Current.MainPage = new NavigationPage(newMainPage);
                    break;
            }
        }

        private async void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (!e.IsConnected)
                await PopupNavigation.PushAsync(new ConnectionLostPopupPage());
            else
            {
                if (PopupNavigation.PopupStack.Any() && PopupNavigation.PopupStack.Last().GetType().Name == "ConnectionLostPopupPage")
                    await PopupNavigation.PopAsync();
            }
        }

        public async static void Logout()
        {
            if (Settings.Type == "a")
                await Current.MainPage.Navigation.PopToRootAsync();
            else
                Current.MainPage = new NavigationPage(new MainPage());
            Settings.ClearSettings();
            Cart.Clear();
            galleryIsOpent = false;
            isConnected = false;

        }

        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public static async Task<bool> CheckConnection()
        {
            while (!CrossConnectivity.Current.IsConnected)
            {
                await App.Current.MainPage.DisplayAlert("Erreur", "Pas de connection internet", "Ressayer");
                return (await CheckConnection());
            }
            return true;
        }

        public static async Task<bool> GetEmailExist(string email)
        {
            if (!(await CheckConnection()) || (App.TokenExpired())) return false;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Token);
            try
            {
                var json = await httpClient.GetStringAsync(App.ServerURL + "api/GetEmailExist/" + email + "/");
                return JsonConvert.DeserializeObject<bool>(json);
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }

        protected async override void OnStart()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await PopupNavigation.PushAsync(new ConnectionLostPopupPage());
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}
