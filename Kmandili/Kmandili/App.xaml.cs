using Kmandili.Models.LocalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Kmandili
{
	public partial class App : Application
	{
        //public static string ServerURL = "http://192.168.1.5:300/";
        public static string ServerURL = "http://seifiisexpress.sytes.net:300/";
        public static Connected Connected = null;
        public static List<CartPastry> Cart = new List<CartPastry>();
        public static bool galleryIsOpent = false;
        public static MainPage Main;
        public App ()
		{
			InitializeComponent();

            Main = new MainPage();
			MainPage = new NavigationPage(Main);
		}

        public async static void Logout()
        {
            Connected = null;
            Cart.Clear();
            galleryIsOpent = false;
            await Main.Navigation.PopModalAsync();
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

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
