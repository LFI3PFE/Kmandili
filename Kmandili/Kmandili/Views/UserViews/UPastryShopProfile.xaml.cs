using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Kmandili.Helpers;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews.PSProductListAndFilter;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UPastryShopProfile
	{
        private readonly ToolbarItem _rateItem;
        private readonly ToolbarItem _submitItem;
        private readonly ToolbarItem _cancelItem;
        private readonly ToolbarItem _productList;
	    private readonly ToolbarItem _pointOfSalesItem;
        private int _starIndex;
        private PastryShop _pastryShop;

        public UPastryShopProfile(PastryShop pastryShop)
		{
            InitializeComponent ();
            _pastryShop = pastryShop;
            ToolbarItems.Clear();
            
            _rateItem = new ToolbarItem
            {
                Icon = "fullStarItem.png",
                Text = "Noter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            _rateItem.Clicked += RateOnClick;

            _productList = new ToolbarItem
            {
                Text = "Produits",
                Order = ToolbarItemOrder.Primary,
                Icon = "products.png",
                Priority = 0
            };
            _productList.Clicked += ProductListOnClick;

            _submitItem = new ToolbarItem
            {
                Icon = "confirm.png",
                Text = "Envoyer",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            _submitItem.Clicked += SubmitOnClick;

            _cancelItem = new ToolbarItem
            {
                Icon = "close.png",
                Text = "Annuler",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            _cancelItem.Clicked += CanelOnClick;

            _pointOfSalesItem = new ToolbarItem
            {
                Text = "Points de vente",
                Order = ToolbarItemOrder.Primary,
                Icon = "shop.png"
            };
            _pointOfSalesItem.Clicked += PointOfSalesItem_Clicked;
#pragma warning restore CS0618 // Type or member is obsolete
            ToolbarItems.Add(_rateItem);
            ToolbarItems.Add(_productList);
            ToolbarItems.Add(_pointOfSalesItem);
            Load(false);
        }

        private async void PointOfSalesItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopPointOfSalesList(_pastryShop));
        }

        private async void Load(bool reload)
        {
            if (reload)
            {
                App.UpdatePastryList = true;
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
                if (_pastryShop == null) return;
            }
            Rating.Text = _pastryShop.Rating.ToString(CultureInfo.InvariantCulture);
            NumberOfReviews.Text = "(" + _pastryShop.Ratings.Count + " avis)";
            Cover.Source = App.ServerUrl + "Uploads/" + _pastryShop.CoverPic;
            ProfilImage.Source = App.ServerUrl + "Uploads/" + _pastryShop.ProfilePic;
            PastryName.Text = _pastryShop.Name;
            Address.Text = _pastryShop.Address.ToString();
            Desc.Text = _pastryShop.LongDesc;
            Email.Text = _pastryShop.Email;
            PriceRange.Text = _pastryShop.PriceRange.MinPriceRange + "-" + _pastryShop.PriceRange.MaxPriceRange;
            PhoneNumbersLayout.Children.Clear();
            foreach (PhoneNumber phone in _pastryShop.PhoneNumbers)
            {
                Grid grid = new Grid
                {
                    RowDefinitions = { new RowDefinition { Height = GridLength.Auto } },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    }
                };
                grid.Children.Add(new Label { Text = phone.Number, TextColor = Color.Black, FontSize = 18 }, 0, 0);
                grid.Children.Add(new Label { Text = phone.PhoneNumberType.Type, TextColor = Color.Black, FontSize = 18 }, 1, 0);
                PhoneNumbersLayout.Children.Add(grid);
            }
            CategoriesLayout.Children.Clear();
            foreach (var category in _pastryShop.Categories)
            {
                CategoriesLayout.Children.Add(new Label { Text = category.CategoryName, TextColor = Color.Black, FontSize = 18 });
            }
            DeleveryMethodsLayout.Children.Clear();
            float height = 0;
            foreach (var pastryShopDeleveryMethod in _pastryShop.PastryShopDeleveryMethods)
            {
                height += 50;
                StackLayout paymentLayout = new StackLayout();
                foreach (var pastryDeleveryPayment in pastryShopDeleveryMethod.PastryDeleveryPayments)
                {
                    height += 40;
                    paymentLayout.Children.Add(new Label
                    {
                        Text = pastryDeleveryPayment.Payment.PaymentMethod,
                        TextColor = Color.Black,
                        FontSize = 15
                    });
                }
                DeleveryMethodsLayout.Children.Add(new StackLayout
                {
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 20,
                            Children =
                            {
                                new Label
                                {
                                    Text = "- " + pastryShopDeleveryMethod.DeleveryMethod.DeleveryType,
                                    TextColor = Color.Black,
                                    FontSize = 18
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Horizontal,
                                    VerticalOptions = LayoutOptions.End,
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = "Delais:",
                                            TextColor = Color.Black,
                                            FontSize = 15,
                                            FontAttributes = FontAttributes.Bold
                                        },
                                        new Label
                                        {
                                            Text = pastryShopDeleveryMethod.DeleveryDelay.ToString(),
                                            TextColor = Color.Black,
                                            FontSize = 15
                                        }
                                    }
                                }
                            }
                        },
                        new StackLayout
                        {
                            Padding = new Thickness(30,0,0,0),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Payment:",
                                    TextColor = Color.Black,
                                    FontSize = 15,
                                    FontAttributes = FontAttributes.Bold
                                },
                                paymentLayout
                            }
                        }
                    }
                });
            }
            DeleveryMethodsLayout.HeightRequest = height;
        }

        //private StackLayout MakePointOfSaleStackLayout(PointOfSale pointOfSale)
        //{
        //    StackLayout MainStack = new StackLayout()
        //    {
        //        BackgroundColor = Color.White,
        //        Margin = new Thickness(0, 0, 0, 7),
        //    };
        //    Grid MainGrid = new Grid();
        //    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
        //    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
        //    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

        //    StackLayout gridFirstStackChild = new StackLayout() { Padding = new Thickness(20, 0, 0, 0) };
        //    StackLayout s1 = new StackLayout()
        //    {
        //        Orientation = StackOrientation.Horizontal
        //    };
        //    s1.Children.Add(new Label() { Text = "Adresse:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
        //    s1.Children.Add(new Label() { Text = pointOfSale.Address.ToString(), FontSize = 15, TextColor = Color.Black });

        //    StackLayout s2 = new StackLayout()
        //    {
        //        Orientation = StackOrientation.Horizontal
        //    };
        //    s2.Children.Add(new Label() { Text = "Parking:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
        //    s2.Children.Add(new Label() { Text = pointOfSale.Parking.ToString(), FontSize = 15, TextColor = Color.Black });

        //    StackLayout s3 = new StackLayout()
        //    {
        //        Orientation = StackOrientation.Horizontal
        //    };
        //    s3.Children.Add(new Label() { Text = "Date de creation:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, WidthRequest = 70 });
        //    s3.Children.Add(new Label() { Text = pointOfSale.CreationDate.Day + "/" + pointOfSale.CreationDate.Month + "/" + pointOfSale.CreationDate.Year, FontSize = 15, TextColor = Color.Black });

        //    StackLayout s4 = new StackLayout();

        //    StackLayout s4Child = new StackLayout() { Padding = new Thickness(10, 0, 0, 0) };
        //    foreach (PhoneNumber ph in pointOfSale.PhoneNumbers)
        //    {
        //        s4Child.Children.Add(new Label() { Text = ph.Number + " " + ph.PhoneNumberType.Type, FontSize = 15, TextColor = Color.Black });
        //    }

        //    s4.Children.Add(new Label() { Text = "Numéro de telephones:", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.Black, HeightRequest = 20 });
        //    s4.Children.Add(s4Child);

        //    gridFirstStackChild.Children.Add(s1);
        //    gridFirstStackChild.Children.Add(s2);
        //    gridFirstStackChild.Children.Add(s3);
        //    gridFirstStackChild.Children.Add(s4);

        //    StackLayout gridSecondStackChild = new StackLayout()
        //    {
        //        Padding = new Thickness(0, 0, 0, 10),
        //    };
        //    Grid TimeTitelGrid = new Grid();
        //    TimeTitelGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        //    TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        //    TimeTitelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
        //    TimeTitelGrid.Children.Add(new Label() { Text = "Jours", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 0, 0);
        //    TimeTitelGrid.Children.Add(new Label() { Text = "Heures", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.Black }, 1, 0);

        //    gridSecondStackChild.Children.Add(TimeTitelGrid);

        //    foreach (WorkDay w in pointOfSale.WorkDays)
        //    {
        //        StackLayout WorkDayStack = new StackLayout();
        //        Grid WorkDayGrid = new Grid();
        //        WorkDayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        //        WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        //        WorkDayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
        //        StackLayout HoursStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
        //        HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.OpenTime), FontSize = 15, TextColor = Color.Green });
        //        HoursStack.Children.Add(new Label() { Text = "-", FontSize = 13, TextColor = Color.Green });
        //        HoursStack.Children.Add(new Label() { Text = TimeSpanToTime(w.CloseTime), FontSize = 13, TextColor = Color.Green });

        //        WorkDayGrid.Children.Add(new Label() { Text = DayNumberToDayName(w.Day), FontSize = 13, TextColor = Color.Black }, 0, 0);
        //        WorkDayGrid.Children.Add(HoursStack, 1, 0);
        //        WorkDayStack.Children.Add(WorkDayGrid);
        //        gridSecondStackChild.Children.Add(WorkDayStack);
        //    }

        //    MainGrid.Children.Add(gridFirstStackChild, 0, 0);
        //    MainGrid.Children.Add(gridSecondStackChild, 1, 0);

        //    MainStack.Children.Add(MainGrid);
        //    return MainStack;
        //}

        //private string TimeSpanToTime(TimeSpan time)
        //{
        //    return time.Hours + "h" + time.Minutes;
        //}

        //private string DayNumberToDayName(int Day)
        //{
        //    switch (Day)
        //    {
        //        case 1:
        //            return "Lun.";
        //        case 2:
        //            return "Mar.";
        //        case 3:
        //            return "Mer.";
        //        case 4:
        //            return "Jeu.";
        //        case 5:
        //            return "Ven.";
        //        case 6:
        //            return "Sam.";
        //        case 7:
        //            return "Dim.";
        //    }
        //    return null;
        //}

        public async void ResetToolbar()
        {
            ReactionLabel.IsVisible = false;
            await RatingLayout.TranslateTo(0, 0);
            RatingLayout.IsVisible = false;
            ToolbarItems.Clear();
            ToolbarItems.Add(_rateItem);
            ToolbarItems.Add(_productList);
            ToolbarItems.Add(_pointOfSalesItem);
            _starIndex = 0;
            var lsi = RatingStack.Children;
            foreach (var view in lsi)
            {
                var i = (Image) view;
                i.Source = "emptyStar.png";
            }
        }

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopProductList(_pastryShop));
        }

        public async void SubmitOnClick(Object sender, EventArgs e)
        {
            if (_starIndex != 0)
            {
                //PastryShop pastryShopTemp = new PastryShop()
                //{
                //    ID = pastryShop.ID,
                //    Name = pastryShop.Name,
                //    Email = pastryShop.Email,
                //    Password = pastryShop.Password,
                //    Address_FK = pastryShop.Address_FK,
                //    ProfilePic = pastryShop.ProfilePic,
                //    CoverPic = pastryShop.CoverPic,
                //    PriceRange_FK = pastryShop.PriceRange_FK,
                //    LongDesc = pastryShop.LongDesc,
                //    ShortDesc = pastryShop.ShortDesc,
                //    NumberOfRatings = ++pastryShop.NumberOfRatings,
                //    RatingSum = pastryShop.RatingSum + starIndex,
                //};

                //var x = pastryShopTemp;
                //PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                //if(!(await pastryShopRC.PutAsync(pastryShop.ID, pastryShopTemp)));
                var rating = _pastryShop.Ratings.FirstOrDefault(r => r.User_FK == Settings.Id);
                if ((rating != null) && (rating.Value != _starIndex))
                {
                    rating.PastryShop = null;
                    rating.User = null;
                    rating.Value = _starIndex;
                    var ratingRc = new RatingRestClient();
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    try
                    {
                        if (!(await ratingRc.PutAsync(rating.User_FK, rating.PastryShop_FK, rating)))
                        {
                            await DisplayAlert("Erreur", "Une erreur s'est produite pendant la mise à jours de votre avis.", "Ok");
                        }
                    }
                    catch (Exception)
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                        await Navigation.PopAsync();
                        return;
                    }
                    
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    rating = new Rating
                    {
                        User_FK = Settings.Id,
                        PastryShop_FK = _pastryShop.ID,
                        Value = _starIndex
                    };
                    var ratingRc = new RatingRestClient();
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    try
                    {
                        if ((await ratingRc.PostAsync(rating)) == null)
                        {
                            await DisplayAlert("Erreur", "Une erreur s'est produite pendant la ajout de votre avis.", "Ok");
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
                }
                Load(true);
                ResetToolbar();
            }
        }

        public void CanelOnClick(Object sender, EventArgs e)
        {
            ResetToolbar();
        }

        public async void RateOnClick(Object sender, EventArgs e)
        {
            var ratingRc = new RatingRestClient();
            try
            {
                var rating = await ratingRc.GetAsyncById(Settings.Id, _pastryShop.ID);
                if (rating != null)
                {
                    ReactionLabel.IsVisible = true;
                    for (var i = 0; i < rating.Value; i++)
                    {
                        ((Image) RatingStack.Children.ElementAt(i)).Source = "fullStar.png";
                    }
                }
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                await Navigation.PopAsync();
            }
            ToolbarItems.Clear();
            ToolbarItems.Add(_submitItem);
            ToolbarItems.Add(_cancelItem);
            RatingLayout.IsVisible = true;
            await RatingLayout.TranslateTo(0, 50);
        }

        public void StarOnClick(Object sender, EventArgs e)
        {
            if (sender is Image x) _starIndex = int.Parse(x.ClassId);
            var lsi = RatingStack.Children;
            foreach (var view in lsi)
            {
                var i = (Image) view;
                i.Source = int.Parse(i.ClassId) <= _starIndex ? "fullStar.png" : "emptyStar.png";
            }
        }

	    private async void RemoveRating(object sender, EventArgs e)
	    {
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        var ratingRc = new RatingRestClient();
	        try
	        {
                if (!(await ratingRc.DeleteAsync(Settings.Id, _pastryShop.ID)))
                {
                    await DisplayAlert("Erreur", "Une erreur s'est produite pendant la suppression de votre avis.", "Ok");
                }
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                await Navigation.PopAsync();
                return;
	        }
	        await PopupNavigation.PopAsync();
            Load(true);
            ResetToolbar();
	    }
    }
}
