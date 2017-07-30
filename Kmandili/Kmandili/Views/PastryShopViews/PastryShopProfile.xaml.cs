using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Views.PastryShopViews.POSListAndAdd;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.ProductListAndFilter;
using Rg.Plugins.Popup.Services;

namespace Kmandili.Views.PastryShopViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProfile : ContentPage
	{
        private ToolbarItem ProductList;
        private ToolbarItem pointOfSaleList;
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
                Icon = "products.png",
                Text = "Produits",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            ProductList.Clicked += ProductListOnClick;

            pointOfSaleList = new ToolbarItem()
            {
                Text = "Points de vente",
                Order = ToolbarItemOrder.Secondary
            };
            pointOfSaleList.Clicked += PointOfSaleList_Clicked;

            ToolbarItems.Add(ProductList);
            ToolbarItems.Add(pointOfSaleList);
            Load();
        }

        private async void PointOfSaleList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PointOfSalesList(pastryShop));
        }

        public async void Reload()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            try
            {
                pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
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
            await PopupNavigation.PopAllAsync();
            if(pastryShop == null) return;
            Load();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void Load()
        {
            Rating.Text = pastryShop.Ratings.Sum(r => r.Value).ToString();
            NumberOfReviews.Text = "(" + pastryShop.Ratings.Count + " avis)";
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
                    RowDefinitions = {new RowDefinition() { Height = GridLength.Auto} },
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

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var pastryShopRC = new PastryShopRestClient();
            try
            {
                pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
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
            await PopupNavigation.PopAllAsync();
            await Navigation.PushAsync(new PSProductList(pastryShop));
        }

        protected override void OnAppearing()
        {
            if (pastryShopMasterDetailPage.hasNavigatedToEdit)
            {
                Reload();
                pastryShopMasterDetailPage.hasNavigatedToEdit = false;
            }
        }
	}
}
