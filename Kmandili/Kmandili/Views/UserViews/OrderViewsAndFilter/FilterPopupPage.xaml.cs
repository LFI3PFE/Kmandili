﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilterPopupPage : PopupPage
	{
	    private UserOrderList userOrderList;
	    private List<Status> selectedStatuses;

	    private List<Status> statuses; 

		public FilterPopupPage (UserOrderList userOrderList, List<Status> selectedStatuses )
		{
            BackgroundColor = Color.FromHex("#CC000000");
            this.userOrderList = userOrderList;
		    this.selectedStatuses = selectedStatuses;
			InitializeComponent ();
		    Load();
		}

	    private async void Load()
	    {
	        RestClient<Status> statusRC = new RestClient<Status>();
	        statuses = await statusRC.GetAsync();
	        if (statuses == null) return;
	        this.Content = MakeContent();
	    }

	    private StackLayout MakeContent()
	    {
            StackLayout mainLayout = new StackLayout()
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            StackLayout innerLayout = new StackLayout()
            {
                Padding = new Thickness(30),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Spacing = 20
            };
            innerLayout.Children.Add(new Label() { Text = "Les status:", FontSize = 20, TextColor = Color.Black, FontAttributes = FontAttributes.Bold });

            StackLayout statusesLayout = new StackLayout() { Spacing = 5 };
            foreach (var status in statuses)
            {
                StackLayout statusLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20, Padding = new Thickness(20, 0, 0, 0) };
                Switch statusSwitch = new Switch()
                {
                    ClassId = status.ID.ToString(),
                };
                statusSwitch.IsToggled = selectedStatuses.Any(s => s.StatusName == status.StatusName);
                statusSwitch.Toggled += StatusSwitch_Toggled;
                statusLayout.Children.Add(statusSwitch);
                statusLayout.Children.Add(new Label() { Text = status.StatusName, FontSize = 18, TextColor = Color.Black, VerticalTextAlignment = TextAlignment.Center });

                statusesLayout.Children.Add(statusLayout);
            }
            innerLayout.Children.Add(statusesLayout);
            Label aplyLabel = new Label() { Text = "Appliquer", TextColor = Color.DodgerBlue, FontSize = 20, HorizontalOptions = LayoutOptions.End };
            TapGestureRecognizer aplyGestureRecognizer = new TapGestureRecognizer();
            aplyGestureRecognizer.Tapped += AplyGestureRecognizer_Tapped;
            aplyLabel.GestureRecognizers.Add(aplyGestureRecognizer);
            innerLayout.Children.Add(aplyLabel);
            mainLayout.Children.Add(innerLayout);

            return mainLayout;
        }

	    private void StatusSwitch_Toggled(object sender, EventArgs e)
	    {
            var statusSwitch = sender as Switch;
            if (statusSwitch.IsToggled)
            {
                selectedStatuses.Add(statuses.FirstOrDefault(s => s.ID == Int32.Parse(statusSwitch.ClassId)));
            }
            else
            {
                selectedStatuses.Remove(selectedStatuses.FirstOrDefault(s => s.ID == Int32.Parse(statusSwitch.ClassId)));
            }
        }


        private async void AplyGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

	    protected override void OnDisappearing()
	    {
            userOrderList.AplyFilters();
	    }
	}
}
