using System;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.Admin.PSViews.Charts;
using Kmandili.Views.Admin.PSViews.Orders;
using Kmandili.Views.PastryShopViews.POSListAndAdd;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using Kmandili.Views.PastryShopViews.ProductListAndFilter;
using Kmandili.Views.PastryShopViews.EditProfile;
using Kmandili.Views.Admin.PSViews.PastryShopListAndFilter;

namespace Kmandili.Views.Admin.PSViews.PSProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class APastryShopProfile
	{
	    private PastryShop _pastryShop;

	    private bool _hasNavigatedToEdit;
	    private bool _updateParent;

	    private readonly APastryShopList _pastryShopList;

        public APastryShopProfile(PastryShop pastryShop, APastryShopList pastryShopList)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            _pastryShopList = pastryShopList;
            var productList = new ToolbarItem
            {
                Text = "Produits",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0
            };
            productList.Clicked += ProductListOnClick;

            var orderToolbarItem = new ToolbarItem()
            {
                Icon = "tbOrders.png",
                Text = "Commandes",
                Order = ToolbarItemOrder.Primary,
            };
            orderToolbarItem.Clicked += OrderToolbarItem_Clicked;

            var pointOfSaleList = new ToolbarItem()
            {
                Text = "Points de vente",
                Order = ToolbarItemOrder.Secondary
            };
            pointOfSaleList.Clicked += PointOfSaleList_Clicked;

            var chartsToolbarItem = new ToolbarItem()
            {
                Text = "Graphiques",
                Order = ToolbarItemOrder.Secondary
            };
            chartsToolbarItem.Clicked += ChartsToolbarItem_Clicked;

            var editToolbarItem = new ToolbarItem()
            {
                Text = "Modifier",
                Order = ToolbarItemOrder.Primary,
                Icon = "edit.png"
            };
            editToolbarItem.Clicked += EditToolbarItem_Clicked;

            ToolbarItems.Add(productList);
            ToolbarItems.Add(editToolbarItem);
            ToolbarItems.Add(pointOfSaleList);
            ToolbarItems.Add(orderToolbarItem);
            ToolbarItems.Add(chartsToolbarItem);
            Reload();
        }

        private async void ChartsToolbarItem_Clicked(object sender, EventArgs e)
        {
            var ordersChart = new OrdersChart(_pastryShop)
            {
                Title = "Commandes"
            };

            var incomsChart = new AEarningsChart(_pastryShop)
            {
                Title = "Revenus"
            };
            var tabbedPage = new TabbedPage()
            {
                Children =
                {
                    ordersChart,
                    incomsChart
                }
            };
            await Navigation.PushAsync(tabbedPage);
        }

        private async void OrderToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ApOrderList(_pastryShop.ID));
        }

        private async void EditToolbarItem_Clicked(object sender, EventArgs e)
        {
            _hasNavigatedToEdit = true;
            await Navigation.PushAsync(new EditProfileInfo(_pastryShop.ID, false));
        }

	    protected override void OnAppearing()
	    {
	        if (_hasNavigatedToEdit)
	        {
	            Reload();
	            _hasNavigatedToEdit = false;
	            _updateParent = true;
	        }
	    }

	    protected override void OnDisappearing()
	    {
	        if (_updateParent)
	        {
	            _pastryShopList.Load();
	            _updateParent = false;
	        }
	    }

        private async void PointOfSaleList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PointOfSalesList(_pastryShop));
        }

        public async void Reload()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
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
            await PopupNavigation.PopAllAsync();
            if (_pastryShop == null) return;
            Load();
        }

        private void Load()
        {
            Rating.Text = _pastryShop.Ratings.Sum(r => r.Value).ToString();
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
            foreach (var category in _pastryShop.Categories)
            {
                CategoriesLayout.Children.Add(new Label() { Text = category.CategoryName, TextColor = Color.Black, FontSize = 18 });
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
            await PopupNavigation.PopAllAsync();
            await Navigation.PushAsync(new PsProductList(_pastryShop));
        }
    }
}
