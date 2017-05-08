using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConnectionLostPopupPage : PopupPage
	{
		public ConnectionLostPopupPage()
		{
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent ();
            StackLayout contentLayout = new StackLayout()
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 20,
                Padding = new Thickness(40,20,40,20),
                Children =
                {
                    new Label()
                    {
                        Text = "Connection perdue...",
                        FontSize = 25,
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    },
                    new ActivityIndicator()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        IsRunning = true,
                        HeightRequest = 30
                    }
                }
            };
		    Content = contentLayout;
		}
	}
}
