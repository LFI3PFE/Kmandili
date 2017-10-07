using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.POSListAndAdd
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PointOfSalesList
	{
	    private PastryShop _pastryShop;

	    public PointOfSalesList (PastryShop pastryShop)
		{
		    _pastryShop = pastryShop;
			InitializeComponent ();
            var addToolbarItem = new ToolbarItem()
            {
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Icon = "plus.png"
            };
		    addToolbarItem.Clicked += AddToolbarItem_OnClicked;
            ToolbarItems.Add(addToolbarItem);
            Load(false);
		}

	    private async void AddToolbarItem_OnClicked(object sender, EventArgs e)
	    {
	        await Navigation.PushAsync(new PointOfSaleForm(this, _pastryShop));
	    }

	    public async void Load(bool reload)
	    {
	        if (reload)
	        {
	            await PopupNavigation.PushAsync(new LoadingPopupPage());
	            var pastryShopRc = new PastryShopRestClient();
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
	            await PopupNavigation.PopAsync();
	            if (_pastryShop == null)return;
	        }
            CoreStackLayout.Children.Clear();
            _pastryShop.PointOfSales.ToList().ForEach(p => CoreStackLayout.Children.Add(MakePointOfSaleStackLayout(p)));
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
            StackLayout s1 = new StackLayout();
            s1.Children.Add(new Label() { Text = "Adresse:", FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.Black});
            s1.Children.Add(new StackLayout()
            {
                Padding = new Thickness(30, 0, 0, 0),
                Children = {new Label() {Text = pointOfSale.Address.ToString(), FontSize = 15, TextColor = Color.Black}}
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
            mainStack.Children.Add(new StackLayout()
            {
                HeightRequest = 2,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Margin = new Thickness(0,30,0,0)
            });
            return mainStack;
        }

	    private async void DeleteTapGesture_Tapped(object sender, EventArgs e)
	    {
	        if (_pastryShop.PointOfSales.Count == 1)
	        {
	            await DisplayAlert("Erreur", "Il faut avoir au moins un point de vente!", "Ok");
                return;
	        }
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
            int id = Int32.Parse(((((((sender as Image)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId);
            PointOfSale pointOfSale = _pastryShop.PointOfSales.FirstOrDefault(p => p.ID == id);
            if (pointOfSale != null)
            {
                RestClient<PointOfSale> pointOfSaleRc = new RestClient<PointOfSale>();
                await PopupNavigation.PopAllAsync();
                try
                {
                    if (!(await pointOfSaleRc.DeleteAsync(pointOfSale.ID))) return;
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
                Load(true);
            }
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
    }
}
