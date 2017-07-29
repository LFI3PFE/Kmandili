﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Views.Admin.PSViews.PastryShopListAndFilter;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdminMasterDetailPage : MasterDetailPage
    {
        private class MyTabbedPage : TabbedPage
        {
            protected override bool OnBackButtonPressed()
            {
                return true;
            }
        }

        public AdminMasterDetailPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            MyTabbedPage tab = new MyTabbedPage()
            {
                Children =
                {
                    new ContentPage()
                    {
#pragma warning disable 618
                        Icon = Device.OnPlatform("shop.png", null, "shop.png"),
                        Title = "Users Dashboard"
                    },
                    new PastryShopList()
                    {
                        Icon = Device.OnPlatform("shop.png", null, "shop.png"),
                        Title = "PastryShops Dashboard"
                    }
#pragma warning restore 618
                    //pastryShopList,
                    //productList
                }
            };
            Master = new AdminMasterPage(this);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.Android:
                    Detail = new NavigationPage(tab);
                    break;
                case Device.WinPhone:
                case Device.Windows:
                    Detail = tab;
                    break;
                default:
                    Detail = new NavigationPage(tab); ;
                    break;
            }
            //IsPresentedChanged += UserMasterDetailPage_IsPresentedChanged;
        }

        //private void UserMasterDetailPage_IsPresentedChanged(object sender, EventArgs e)
        //{
        //    if (IsPresented)
        //        (Master as AdminMasterPage).UpdateOrderNotificationNumber();
        //}
    }
}