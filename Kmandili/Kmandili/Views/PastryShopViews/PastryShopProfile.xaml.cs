using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.ProductListAndFilter;

namespace Kmandili.Views.PastryShopViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProfile : ContentPage
	{
        private ToolbarItem ProductList;
        private PastryShop pastryShop;
        private PastryShopMasterDetailPage pastryShopMasterDetailPage;

        public PastryShopProfile(PastryShopMasterDetailPage pastryShopMasterDetailPage, PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
            //ToolbarItems.Clear();
            ProductList = new ToolbarItem
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Icon = Device.OnPlatform(null, null, "products.png"),
#pragma warning restore CS0618 // Type or member is obsolete
                Text = "Produits",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            ProductList.Clicked += ProductListOnClick;

            ToolbarItems.Add(ProductList);
            RefreshRating();
            load();
        }

        public async void Reload()
        {
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            load();
        }

        protected override void OnAppearing()
        {
            pastryShopMasterDetailPage.IsGestureEnabled = true;
        }

        protected override void OnDisappearing()
        {
            pastryShopMasterDetailPage.IsGestureEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void load()
        {
            Cover.Source = pastryShop.CoverPic;
            ProfilImage.Source = pastryShop.ProfilePic;
            PastryName.Text = pastryShop.Name;
            Address.Text = pastryShop.Address.ToString();
            Desc.Text = pastryShop.LongDesc;
            Email.Text = pastryShop.Email;
            PhoneNumbersLayout.Children.Clear();
            foreach (PhoneNumber phone in pastryShop.PhoneNumbers)
            {
                PhoneNumbersLayout.Children.Add(new Label() { Text = phone.Number + " " + phone.PhoneNumberType.Type, TextColor = Color.Black, FontSize = 20 });
            }
            CoreStackLayout.Children.Clear();
            foreach (PointOfSale pointOfSale in pastryShop.PointOfSales)
            {
                CoreStackLayout.Children.Add(MakePointOfSaleStackLayout(pointOfSale));
            }
        }

        private StackLayout MakePointOfSaleStackLayout(PointOfSale pointOfSale)
        {
            StackLayout MainStack = new StackLayout()
            {
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 0, 0, 7),
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
            s1.Children.Add(new Label() { Text = "Adresse:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
            s1.Children.Add(new Label() { Text = pointOfSale.Address.ToString(), FontSize = 15, TextColor = Color.Black });

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
            s3.Children.Add(new Label() { Text = pointOfSale.CreationDate.Day + "/" + pointOfSale.CreationDate.Month + "/" + pointOfSale.CreationDate.Year, FontSize = 15, TextColor = Color.Black });

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
            Grid TimeTitelGrid = new Grid();
            TimeTitelGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            TimeTitelGrid.Children.Add(new Label() { Text = "Jours", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 0, 0);
            TimeTitelGrid.Children.Add(new Label() { Text = "Heures", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 1, 0);

            gridSecondStackChild.Children.Add(TimeTitelGrid);

            foreach (WorkDay w in pointOfSale.WorkDays)
            {
                StackLayout WorkDayStack = new StackLayout();
                Grid WorkDayGrid = new Grid();
                WorkDayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                StackLayout HoursStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.OpenTime), FontSize = 15, TextColor = Color.Green });
                HoursStack.Children.Add(new Label() { Text = "-", FontSize = 13, TextColor = Color.Green });
                HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.CloseTime), FontSize = 13, TextColor = Color.Green });

                WorkDayGrid.Children.Add(new Label() { Text = DayNumberToDayName(w.Day), FontSize = 13, TextColor = Color.Black }, 0, 0);
                WorkDayGrid.Children.Add(HoursStack, 1, 0);
                WorkDayStack.Children.Add(WorkDayGrid);
                gridSecondStackChild.Children.Add(WorkDayStack);
            }

            MainGrid.Children.Add(gridFirstStackChild, 0, 0);
            MainGrid.Children.Add(gridSecondStackChild, 1, 0);

            MainStack.Children.Add(MainGrid);
            return MainStack;
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

        public async void RefreshRating()
        {
            PastryShopRestClient rs = new PastryShopRestClient();
            PastryShop pp = await rs.GetAsyncById(pastryShop.ID);
            pastryShop = pp;
            Rating.Text = pastryShop.Rating.ToString();
            NumberOfReviews.Text = "(" + pastryShop.NumberOfRatings.ToString() + " avis)";
        }

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PSProductList(pastryShop));
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.WinPhone:
            //        await Navigation.PushModalAsync(new NavigationPage(new PastryShopProductList(pastryShop)));
            //        break;
            //    case Device.Windows:
            //        NavigationPage nav = new NavigationPage(new ContentPage());
            //        await nav.PushAsync(new PastryShopProductList(pastryShop));
            //        await Navigation.PushModalAsync(nav);
            //        break;
            //    default:
            //        await Navigation.PushAsync(new PastryShopProductList(pastryShop));
            //        break;
            //}
        }

    }
}
