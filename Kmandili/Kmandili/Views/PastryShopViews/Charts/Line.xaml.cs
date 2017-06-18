﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models;
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
	    private int year;
	    private int semester;
	    private DateTime max, min;

		public Line (PastryShop pastryShop)
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
            var htmlWebView = new HtmlWebViewSource()
            {
                Html = await chartRC.GetChartView(App.ServerURL + "api/GetLineChartView/" + Settings.Id+"/"+year+"/"+semester)
            };
            Browser.Source = htmlWebView;
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