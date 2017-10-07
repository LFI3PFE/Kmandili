using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.OrderViewsAndFilter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class POrderFilterPopupPage
    {
        private readonly PsOrderList _pastryShopOrderList;
        private readonly List<Status> _selectedStatuses;

        private List<Status> _statuses;

        public POrderFilterPopupPage(PsOrderList pastryShopOrderList, List<Status> selectedStatuses)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            _pastryShopOrderList = pastryShopOrderList;
            _selectedStatuses = selectedStatuses;
            InitializeComponent();
            Load();
        }

        private async void Load()
        {
            RestClient<Status> statusRc = new RestClient<Status>();
            try
            {
                _statuses = await statusRc.GetAsync();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                return;
            }
            if (_statuses == null) return;
            Content = MakeContent();
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
            innerLayout.Children.Add(new Label()
            {
                Text = "Les status:",
                FontSize = 20,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold
            });

            StackLayout statusesLayout = new StackLayout() {Spacing = 5};
            foreach (var status in _statuses)
            {
                StackLayout statusLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 20,
                    Padding = new Thickness(20, 0, 0, 0)
                };
                Switch statusSwitch = new Switch
                {
                    ClassId = status.ID.ToString(),
                    IsToggled = _selectedStatuses.Any(s => s.StatusName == status.StatusName),
                };
                statusSwitch.Toggled += StatusSwitch_Toggled;
                statusLayout.Children.Add(statusSwitch);
                statusLayout.Children.Add(new Label()
                {
                    Text = status.StatusName,
                    FontSize = 18,
                    TextColor = Color.Black,
                    VerticalTextAlignment = TextAlignment.Center
                });

                statusesLayout.Children.Add(statusLayout);
            }
            innerLayout.Children.Add(statusesLayout);
            Label aplyLabel = new Label()
            {
                Text = "Appliquer",
                TextColor = Color.DodgerBlue,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.End
            };
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
            if (statusSwitch != null && statusSwitch.IsToggled)
            {
                _selectedStatuses.Add(_statuses.FirstOrDefault(s => s.ID == Int32.Parse(statusSwitch.ClassId)));
            }
            else
            {
                _selectedStatuses.Remove(_selectedStatuses.FirstOrDefault(s => s.ID == Int32.Parse(statusSwitch?.ClassId)));
            }
        }


        private async void AplyGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            _pastryShopOrderList.AplyFilters();
        }
    }
}
