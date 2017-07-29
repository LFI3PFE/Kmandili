using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews;
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
	public partial class PastryShopProfile : ContentPage
	{
        private ToolbarItem ProductList;
        private ToolbarItem pointOfSaleList;
	    private ToolbarItem editToolbarItem;
        private PastryShop pastryShop;

	    private bool hasNavigatedToEdit = false;
	    private bool updateParent = false;

	    private PastryShopList pastryShopList;

        public PastryShopProfile(PastryShop pastryShop, PastryShopList pastryShopList)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.pastryShopList = pastryShopList;
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

            editToolbarItem = new ToolbarItem()
            {
                Text = "Modifier",
                Order = ToolbarItemOrder.Primary,
                Icon = "edit.png"
            };
            editToolbarItem.Clicked += EditToolbarItem_Clicked;

            ToolbarItems.Add(ProductList);
            ToolbarItems.Add(editToolbarItem);
            ToolbarItems.Add(pointOfSaleList);
            Load();
        }

        private async void EditToolbarItem_Clicked(object sender, EventArgs e)
        {
            hasNavigatedToEdit = true;
            await Navigation.PushAsync(new EditProfileInfo(pastryShop.ID));
        }

	    protected override void OnAppearing()
	    {
	        if (hasNavigatedToEdit)
	        {
	            Reload();
	            hasNavigatedToEdit = false;
	            updateParent = true;
	        }
	    }

	    protected override void OnDisappearing()
	    {
	        if (updateParent)
	        {
	            pastryShopList.load();
	            updateParent = false;
	        }
	    }

        private async void PointOfSaleList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PointOfSalesList(pastryShop));
        }

        public async void Reload()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            await PopupNavigation.PopAllAsync();
            if (pastryShop == null) return;
            Load();
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

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            await PopupNavigation.PopAllAsync();
            await Navigation.PushAsync(new PSProductList(pastryShop));
        }
    }
}
