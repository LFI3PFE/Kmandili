using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.RangeSlider.Common;
using Xamarin.RangeSlider.Forms;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PProductFilterPopupPage
	{
        private readonly List<Category> _selectedCategories;
        private List<Category> _categories;
        private readonly PsProductList _productList;
        private readonly PriceRange _selectedpriceRange;
        private readonly PriceRange _maxPriceRange;
        public PProductFilterPopupPage(PsProductList productList, List<Category> selectedCategories, PriceRange maxPriceRange, PriceRange selectedPriceRange)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            _maxPriceRange = maxPriceRange;
            _selectedpriceRange = selectedPriceRange;
            _selectedCategories = selectedCategories;
            _productList = productList;
            InitializeComponent();
            Load();
        }

        private async void Load()
        {
            RestClient<Category> categorieRc = new RestClient<Category>();
            try
            {
                _categories = await categorieRc.GetAsync();
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
            if (_categories == null) return;
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
            innerLayout.Children.Add(new Label() { Text = "Les catégories:", FontSize = 20, TextColor = Color.Black, FontAttributes = FontAttributes.Bold });

            StackLayout categoriesLayout = new StackLayout() { Spacing = 5 };
            foreach (var category in _categories)
            {
                StackLayout categoryLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20, Padding = new Thickness(20, 0, 0, 0) };
                Switch categorySwitch = new Switch
                {
                    ClassId = category.ID.ToString(),
                    IsToggled = _selectedCategories.Any(sc => sc.CategoryName == category.CategoryName),
                };
                categorySwitch.Toggled += CategorySwitch_Toggled;
                categoryLayout.Children.Add(categorySwitch);
                categoryLayout.Children.Add(new Label() { Text = category.CategoryName, FontSize = 18, TextColor = Color.Black, VerticalTextAlignment = TextAlignment.Center });

                categoriesLayout.Children.Add(categoryLayout);
            }
            innerLayout.Children.Add(categoriesLayout);
            innerLayout.Children.Add(new Label() { Text = "Plage de prix:", FontSize = 20, TextColor = Color.Black, FontAttributes = FontAttributes.Bold });
            StackLayout priceRangeLayout = new StackLayout() { Padding = new Thickness(20, 0, 0, 0) };
            if (_maxPriceRange.MinPriceRange != _maxPriceRange.MaxPriceRange)
            {
                RangeSlider priceRangeSlider = new RangeSlider() { MinimumValue = (float)_maxPriceRange.MinPriceRange, MaximumValue = (float)_maxPriceRange.MaxPriceRange, StepValue = 0.00f, LowerValue = (float)_selectedpriceRange.MinPriceRange, UpperValue = (float)_selectedpriceRange.MaxPriceRange, ShowTextAboveThumbs = true, TextSize = 20, FormatLabel = FormaLabel };
                priceRangeSlider.DragCompleted += PriceRangeSlider_DragCompleted;
                priceRangeLayout.Children.Add(priceRangeSlider);
            }
            else
            {
                priceRangeLayout.Children.Add(new Label() { Text = "Un seul prix: " + _maxPriceRange.MinPriceRange + "dt", HorizontalTextAlignment = TextAlignment.Center });
            }
            innerLayout.Children.Add(priceRangeLayout);
            Label aplyLabel = new Label() { Text = "Appliquer", TextColor = Color.DodgerBlue, FontSize = 20, HorizontalOptions = LayoutOptions.End };
            TapGestureRecognizer aplyGestureRecognizer = new TapGestureRecognizer();
            aplyGestureRecognizer.Tapped += AplyGestureRecognizer_Tapped;
            aplyLabel.GestureRecognizers.Add(aplyGestureRecognizer);
            innerLayout.Children.Add(aplyLabel);
            mainLayout.Children.Add(innerLayout);

            return mainLayout;
        }

        private void PriceRangeSlider_DragCompleted(object sender, EventArgs e)
        {
            _selectedpriceRange.MinPriceRange = Math.Ceiling(((RangeSlider) sender).LowerValue * 2) / 2;
            _selectedpriceRange.MaxPriceRange = Math.Ceiling(((RangeSlider) sender).UpperValue * 2) / 2;
        }

        private string FormaLabel(Thumb thumb, float val)
        {
            return Math.Ceiling(val * 2) / 2 + " TND";
        }

        private async void AplyGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private void CategorySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var categorySwitch = sender as Switch;
            if (categorySwitch != null && categorySwitch.IsToggled)
            {
                _selectedCategories.Add(_categories.FirstOrDefault(c => c.ID == Int32.Parse(categorySwitch.ClassId)));
            }
            else
            {
                _selectedCategories.Remove(_selectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(categorySwitch.ClassId)));
            }
        }

        protected override void OnDisappearing()
        {
            _productList.AplyFilters();
        }
    }
}
