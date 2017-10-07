using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopEnteringPointOfSales
	{
        private PastryShop _pastryShop;
        public PastryShopEnteringPointOfSales(PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
        }

        private StackLayout MakePointOfSaleStackLayout(PointOfSale pointOfSale)
        {
            StackLayout mainStack = new StackLayout()
            {
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 0, 0, 7),
                ClassId = pointOfSale.ID.ToString(),
            };
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

            StackLayout gridFirstStackChild = new StackLayout() { Padding = new Thickness(20, 0, 0, 0) };
            StackLayout s1 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            s1.Children.Add(new Label() { Text = "Adresse:", FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
            s1.Children.Add(new StackLayout()
            {
                Padding = new Thickness(30, 0, 0, 0),
                Children = { new Label() { Text = pointOfSale.Address.ToString(), FontSize = 15, TextColor = Color.Black } }
            });

            StackLayout s2 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            s2.Children.Add(new Label() { Text = "Parking:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
            s2.Children.Add(new Label() { Text = pointOfSale.Parking.ToString(), FontSize = 15, TextColor = Color.Black });

            StackLayout s3 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            s3.Children.Add(new Label() { Text = "Date de creation:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
            s3.Children.Add(new Label() { Text = pointOfSale.CreationDate.Day + "/" + pointOfSale.CreationDate.Month + "/" + pointOfSale.CreationDate.Year, FontSize = 15, TextColor = Color.Black, VerticalOptions = LayoutOptions.Center });

            StackLayout s4 = new StackLayout();

            StackLayout s4Child = new StackLayout() { Padding = new Thickness(10, 0, 0, 0) };
            foreach (PhoneNumber ph in pointOfSale.PhoneNumbers)
            {
                s4Child.Children.Add(new Label() { Text = ph.Number + " " + ph.PhoneNumberType.Type, FontSize = 15, TextColor = Color.Black });
            }

            s4.Children.Add(new Label() { Text = "Numéro de telephones:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, HeightRequest = 20 });
            s4.Children.Add(s4Child);

            gridFirstStackChild.Children.Add(s1);
            gridFirstStackChild.Children.Add(s2);
            gridFirstStackChild.Children.Add(s3);
            gridFirstStackChild.Children.Add(s4);

            StackLayout gridSecondStackChild = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 10),
            };
            Image deleteIcon = new Image()
            {
                Source = "delete.png",
                HeightRequest = 22
            };
            StackLayout deleteLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            TapGestureRecognizer deleteTapGesture = new TapGestureRecognizer();
            deleteTapGesture.Tapped += DeleteTapGesture_Tapped;
            deleteIcon.GestureRecognizers.Add(deleteTapGesture);

            deleteLayout.Children.Add(deleteIcon);

            Grid timeTitelGrid = new Grid();
            timeTitelGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            timeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            timeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            timeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            timeTitelGrid.Children.Add(new Label() { Text = "Jours", FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 0, 0);
            timeTitelGrid.Children.Add(new Label() { Text = "Heures", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 1, 0);
            timeTitelGrid.Children.Add(deleteLayout, 2, 0);

            gridSecondStackChild.Children.Add(timeTitelGrid);

            foreach (WorkDay w in pointOfSale.WorkDays)
            {
                StackLayout workDayStack = new StackLayout();
                Grid workDayGrid = new Grid();
                workDayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                workDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                workDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) });
                StackLayout hoursStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                hoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.OpenTime), FontSize = 15, TextColor = Color.Green });
                hoursStack.Children.Add(new Label() { Text = "-", FontSize = 15, TextColor = Color.Green });
                hoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.CloseTime), FontSize = 15, TextColor = Color.Green });

                workDayGrid.Children.Add(new Label() { Text = DayNumberToDayName(w.Day), FontSize = 15, TextColor = Color.Black }, 0, 0);
                workDayGrid.Children.Add(hoursStack, 1, 0);
                workDayStack.Children.Add(workDayGrid);
                gridSecondStackChild.Children.Add(workDayStack);
            }

            mainGrid.Children.Add(gridFirstStackChild, 0, 0);
            mainGrid.Children.Add(gridSecondStackChild, 1, 0);

            mainStack.Children.Add(mainGrid);
            return mainStack;
        }

        private async void DeleteTapGesture_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            int id = Int32.Parse(((((((sender as Image)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId);
            PointOfSale pointOfSale = _pastryShop.PointOfSales.FirstOrDefault(p => p.ID == id);
            if (pointOfSale == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            RestClient<PointOfSale> pointOfSaleRc = new RestClient<PointOfSale>();
            try
            {
                if (!(await pointOfSaleRc.DeleteAsync(pointOfSale.ID)))
                {
                    await PopupNavigation.PopAsync();
                    return;
                }
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
                return;
            }
            
            await PopupNavigation.PopAsync();
            Load();
        }

        private string TimeSpanToTime(TimeSpan time)
        {
            return time.Hours + "h" + time.Minutes;
        }

        private string DayNumberToDayName(int day)
        {
            switch (day)
            {
                case 1:
                    return "Lun.";
                case 2:
                    return "Mar.";
                case 3:
                    return "Mer.";
                case 4:
                    return "Jeu.";
                case 5:
                    return "Ven.";
                case 6:
                    return "Sam.";
                case 7:
                    return "Dim.";
            }
            return null;
        }

        public async void Load()
        {
            CoreStackLayout.Children.Clear();
            NoResultsLabel.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            AddBt.IsEnabled = false;
            ContinueBt.IsEnabled = false;
            PastryShopRestClient pastryShopRc = new PastryShopRestClient();
            try
            {
                _pastryShop = await pastryShopRc.GetAsyncById(_pastryShop.ID);
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
                return;
            }
            if(_pastryShop == null) return;
            ContinueBt.IsEnabled = true;
            if (_pastryShop == null || _pastryShop.PointOfSales.Count == 0)
            {
                NoResultsLabel.IsVisible = true;
                ContinueBt.IsEnabled = false;
            }
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            AddBt.IsEnabled = true;
            foreach (PointOfSale p in _pastryShop.PointOfSales)
            {
                CoreStackLayout.Children.Add(MakePointOfSaleStackLayout(p));
            }
        }

        public async void DeletePointOfSale(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            int id = Int32.Parse((((sender as Image)?.Parent as StackLayout)?.Children[0] as Label)?.Text);
            RestClient<PointOfSale> pointOfSaleRc = new RestClient<PointOfSale>();
            try
            {
                if (!(await pointOfSaleRc.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAsync();
                    return;
                }
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
                return;
            }
            
            await PopupNavigation.PopAsync();
            Load();
        }

        public async void AddProduct_OnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopPointOfSaleForm(this, _pastryShop));
        }

        protected override void OnAppearing()
        {
            Load();
        }

        public async void Continue(Object sender, EventArgs e)
        {
            if (_pastryShop.PointOfSales.Count != 0)
            {
                var authorizationRestClient = new AuthorizationRestClient();
                try
                {
                    var tokenResponse =
                    await authorizationRestClient.AuthorizationLoginAsync(_pastryShop.Email, _pastryShop.Password);
                    if (tokenResponse == null) return;
                    Settings.SetSettings(_pastryShop.Email, _pastryShop.Password, _pastryShop.ID, tokenResponse.access_token, tokenResponse.Type, tokenResponse.expires);
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
                catch (HttpRequestException)
                {
                    await PopupNavigation.PopAllAsync();
                    await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Au moins un point de vente doit être entré!", "Ok");
            }
        }

        public void SelectedNot(Object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}
