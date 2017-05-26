using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopEnteringPointOfSales : ContentPage
	{
        private PastryShop pastryShop;
        public PastryShopEnteringPointOfSales(PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
        }

        private StackLayout MakePointOfSaleStackLayout(PointOfSale pointOfSale)
        {
            StackLayout MainStack = new StackLayout()
            {
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 0, 0, 7),
                ClassId = pointOfSale.ID.ToString(),
            };
            Grid MainGrid = new Grid();
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

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

            Grid TimeTitelGrid = new Grid();
            TimeTitelGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            TimeTitelGrid.Children.Add(new Label() { Text = "Jours", FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 0, 0);
            TimeTitelGrid.Children.Add(new Label() { Text = "Heures", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 1, 0);
            TimeTitelGrid.Children.Add(deleteLayout, 2, 0);

            gridSecondStackChild.Children.Add(TimeTitelGrid);

            foreach (WorkDay w in pointOfSale.WorkDays)
            {
                StackLayout WorkDayStack = new StackLayout();
                Grid WorkDayGrid = new Grid();
                WorkDayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) });
                StackLayout HoursStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.OpenTime), FontSize = 15, TextColor = Color.Green });
                HoursStack.Children.Add(new Label() { Text = "-", FontSize = 15, TextColor = Color.Green });
                HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.CloseTime), FontSize = 15, TextColor = Color.Green });

                WorkDayGrid.Children.Add(new Label() { Text = DayNumberToDayName(w.Day), FontSize = 15, TextColor = Color.Black }, 0, 0);
                WorkDayGrid.Children.Add(HoursStack, 1, 0);
                WorkDayStack.Children.Add(WorkDayGrid);
                gridSecondStackChild.Children.Add(WorkDayStack);
            }

            MainGrid.Children.Add(gridFirstStackChild, 0, 0);
            MainGrid.Children.Add(gridSecondStackChild, 1, 0);

            MainStack.Children.Add(MainGrid);
            return MainStack;
        }

        private async void DeleteTapGesture_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            int ID = Int32.Parse(((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId);
            PointOfSale pointOfSale = pastryShop.PointOfSales.FirstOrDefault(p => p.ID == ID);
            if (pointOfSale == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            RestClient<PointOfSale> pointOfSaleRC = new RestClient<PointOfSale>();
            if (!(await pointOfSaleRC.DeleteAsync(pointOfSale.ID)))
            {
                await PopupNavigation.PopAsync();
                return;
            }
            await PopupNavigation.PopAsync();
            load();
        }

        private string TimeSpanToTime(TimeSpan time)
        {
            return time.Hours + "h" + time.Minutes;
        }

        private string DayNumberToDayName(int Day)
        {
            switch (Day)
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

        public async void load()
        {
            CoreStackLayout.Children.Clear();
            NoResultsLabel.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            AddBt.IsEnabled = false;
            ContinueBt.IsEnabled = false;
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            if(pastryShop == null) return;
            ContinueBt.IsEnabled = true;
            if (pastryShop == null || pastryShop.PointOfSales.Count == 0)
            {
                NoResultsLabel.IsVisible = true;
                ContinueBt.IsEnabled = false;
            }
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            AddBt.IsEnabled = true;
            foreach (PointOfSale p in pastryShop.PointOfSales)
            {
                CoreStackLayout.Children.Add(MakePointOfSaleStackLayout(p));
            }
        }

        public async void DeletePointOfSale(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            int ID = Int32.Parse((((sender as Image).Parent as StackLayout).Children[0] as Label).Text);
            RestClient<PointOfSale> pointOfSaleRC = new RestClient<PointOfSale>();
            if (!(await pointOfSaleRC.DeleteAsync(ID)))
            {
                await PopupNavigation.PopAsync();
                return;
            }
            await PopupNavigation.PopAsync();
            load();
        }

        public async void AddProduct_OnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopPointOfSaleForm(this, pastryShop));
        }

        protected override void OnAppearing()
        {
            load();
        }

        public async void Continue(Object sender, EventArgs e)
        {
            if (pastryShop.PointOfSales.Count != 0)
            {
                //var page = new MainPage();
                //page.SignInAction(pastryShop.Email, pastryShop.Password);
                var authorizationRestClient = new AuthorizationRestClient();
                var tokenResponse =
                    await authorizationRestClient.AuthorizationLoginAsync(pastryShop.Email, pastryShop.Password);
                Settings.SetSettings(pastryShop.Email, pastryShop.Password, pastryShop.ID, tokenResponse.access_token, tokenResponse.Type, tokenResponse.expires);
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await DisplayAlert("Erreur", "Au moins un point de vente doit être entré!", "Ok");
            }
        }

        public void SelectedNot(Object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
