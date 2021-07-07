using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.UserViews.PSProductListAndFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProfile : ContentPage
	{
        private ToolbarItem RateItem;
        private ToolbarItem submitItem;
        private ToolbarItem cancelItem;
        private ToolbarItem ProductList;
	    private ToolbarItem PointOfSalesItem;
        private int starIndex = 0;
        private PastryShop pastryShop;

        public PastryShopProfile (PastryShop pastryShop)
		{
            InitializeComponent ();
            this.pastryShop = pastryShop;
            ToolbarItems.Clear();
            
            RateItem = new ToolbarItem
            {
                Icon = "fullStarItem.png",
                Text = "Noter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            RateItem.Clicked += RateOnClick;

            ProductList = new ToolbarItem
            {
                Icon = "products.png",
                Text = "Produits",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            ProductList.Clicked += ProductListOnClick;

            submitItem = new ToolbarItem
            {
                Icon = "confirm.png",
                Text = "Envoyer",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            submitItem.Clicked += SubmitOnClick;

            cancelItem = new ToolbarItem
            {
                Icon = "close.png",
                Text = "Annuler",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            cancelItem.Clicked += CanelOnClick;

            PointOfSalesItem = new ToolbarItem()
            {
                Text = "Points de vente",
                Order = ToolbarItemOrder.Secondary
            };
            PointOfSalesItem.Clicked += PointOfSalesItem_Clicked;
#pragma warning restore CS0618 // Type or member is obsolete
            ToolbarItems.Add(RateItem);
            ToolbarItems.Add(ProductList);
            ToolbarItems.Add(PointOfSalesItem);
            Load(false);
        }

        private async void PointOfSalesItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopPointOfSalesList(pastryShop));
        }

        private async void Load(bool reload)
        {
            if (reload)
            {
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                pastryShop = await pastryShopRC.GetAsyncById(Settings.Id);
                if (pastryShop == null) return;
            }
            Rating.Text = pastryShop.Rating.ToString();
            NumberOfReviews.Text = "(" + pastryShop.NumberOfRatings.ToString() + " avis)";
            Cover.Source = pastryShop.CoverPic;
            ProfilImage.Source = pastryShop.ProfilePic;
            PastryName.Text = pastryShop.Name;
            Address.Text = pastryShop.Address.ToString();
            Desc.Text = pastryShop.LongDesc;
            Email.Text = pastryShop.Email;
            PriceRange.Text = pastryShop.PriceRange.MinPriceRange + "-" + pastryShop.PriceRange.MaxPriceRange;
            PhoneNumbersLayout.Children.Clear();
            foreach (PhoneNumber phone in pastryShop.PhoneNumbers)
            {
                Grid grid = new Grid()
                {
                    RowDefinitions = { new RowDefinition() { Height = GridLength.Auto } },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                    }
                };
                grid.Children.Add(new Label() { Text = phone.Number, TextColor = Color.Black, FontSize = 18 }, 0, 0);
                grid.Children.Add(new Label() { Text = phone.PhoneNumberType.Type, TextColor = Color.Black, FontSize = 18 }, 1, 0);
                PhoneNumbersLayout.Children.Add(grid);
            }
            CategoriesLayout.Children.Clear();
            foreach (var category in pastryShop.Categories)
            {
                CategoriesLayout.Children.Add(new Label() { Text = category.CategoryName, TextColor = Color.Black, FontSize = 18 });
            }
            DeleveryMethodsLayout.Children.Clear();
            float height = 0;
            foreach (var pastryShopDeleveryMethod in pastryShop.PastryShopDeleveryMethods)
            {
                height += 50;
                StackLayout paymentLayout = new StackLayout();
                foreach (var pastryDeleveryPayment in pastryShopDeleveryMethod.PastryDeleveryPayments)
                {
                    height += 40;
                    paymentLayout.Children.Add(new Label()
                    {
                        Text = pastryDeleveryPayment.Payment.PaymentMethod,
                        TextColor = Color.Black,
                        FontSize = 15
                    });
                }
                DeleveryMethodsLayout.Children.Add(new StackLayout()
                {
                    Children =
                    {
                        new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 20,
                            Children =
                            {
                                new Label()
                                {
                                    Text = "- " + pastryShopDeleveryMethod.DeleveryMethod.DeleveryType,
                                    TextColor = Color.Black,
                                    FontSize = 18
                                },
                                new StackLayout()
                                {
                                    Orientation = StackOrientation.Horizontal,
                                    VerticalOptions = LayoutOptions.End,
                                    Children =
                                    {
                                        new Label()
                                        {
                                            Text = "Delais:",
                                            TextColor = Color.Black,
                                            FontSize = 15,
                                            FontAttributes = FontAttributes.Bold
                                        },
                                        new Label()
                                        {
                                            Text = pastryShopDeleveryMethod.DeleveryDelay.ToString(),
                                            TextColor = Color.Black,
                                            FontSize = 15
                                        }
                                    }
                                }
                            }
                        },
                        new StackLayout()
                        {
                            Padding = new Thickness(30,0,0,0),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                new Label()
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
            await RatingLayout.TranslateTo(0, 0);
            RatingLayout.IsVisible = false;
            ToolbarItems.Clear();
            ToolbarItems.Add(RateItem);
            ToolbarItems.Add(ProductList);
            ToolbarItems.Add(PointOfSalesItem);
            starIndex = 0;
            var lsi = RatingStack.Children;
            foreach (Image i in lsi)
            {
                i.Source = "emptyStar.png";
            }
            ReactionLabel.Text = "";
        }

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopProductList(pastryShop));
        }

        public async void SubmitOnClick(Object sender, EventArgs e)
        {
            if (starIndex != 0)
            {
                PastryShop pastryShopTemp = new PastryShop()
                {
                    ID = pastryShop.ID,
                    Name = pastryShop.Name,
                    Email = pastryShop.Email,
                    Password = pastryShop.Password,
                    Address_FK = pastryShop.Address_FK,
                    ProfilePic = pastryShop.ProfilePic,
                    CoverPic = pastryShop.CoverPic,
                    PriceRange_FK = pastryShop.PriceRange_FK,
                    LongDesc = pastryShop.LongDesc,
                    ShortDesc = pastryShop.ShortDesc,
                    NumberOfRatings = ++pastryShop.NumberOfRatings,
                    RatingSum = pastryShop.RatingSum + starIndex,
                };

                var x = pastryShopTemp;
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                if(!(await pastryShopRC.PutAsync(pastryShop.ID, pastryShopTemp)));
                Load(true);
                ResetToolbar();
            }
            else
            {
                ReactionLabel.Text = "Select a star!";
            }
        }

        public void CanelOnClick(Object sender, EventArgs e)
        {
            ResetToolbar();
        }

        public void RateOnClick(Object sender, EventArgs e)
        {
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(submitItem);
            this.ToolbarItems.Add(cancelItem);
            RatingLayout.IsVisible = true;
            RatingLayout.TranslateTo(0, 50);
        }

        private string hate_string = "Je déteste";
        private string dislike_string = "J'aime pas";
        private string ok_string = "Pas mal";
        private string like_string = "J'aime";
        private string love_string = "J'adore ";

        public void starOnClick(Object sender, EventArgs e)
        {
            var x = sender as Image;
            starIndex = int.Parse(x.ClassId);
            switch (starIndex)
            {
                case 1:
                    ReactionLabel.Text = hate_string;
                    ReactionLabel.TextColor = Color.Black;
                    break;
                case 2:
                    ReactionLabel.Text = dislike_string;
                    ReactionLabel.TextColor = Color.Black;
                    break;
                case 3:
                    ReactionLabel.Text = ok_string;
                    ReactionLabel.TextColor = Color.Black;
                    break;
                case 4:
                    ReactionLabel.Text = like_string;
                    ReactionLabel.TextColor = Color.Black;
                    break;
                case 5:
                    ReactionLabel.Text = love_string;
                    ReactionLabel.TextColor = Color.FromHex("#c84f3b");
                    break;
            }
            var lsi = RatingStack.Children;
            foreach (Image i in lsi)
            {
                if (int.Parse(i.ClassId) <= starIndex)
                {
                    i.Source = "fullStar.png";
                }
                else
                {
                    i.Source = "emptyStar.png";
                }
            }
        }
    }
}
