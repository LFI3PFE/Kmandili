using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.RangeSlider.Common;
using Xamarin.RangeSlider.Forms;

namespace Kmandili.Views.UserViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilerPopupPage : PopupPage
	{
        private List<Category> selectedCategories;
        private List<Category> categories;
        private ProductList productList;
	    private PriceRange selectedpriceRange;
	    private PriceRange maxPriceRange;
        public FilerPopupPage(ProductList productList, List<Category> selectedCategories, PriceRange maxPriceRange, PriceRange selectedPriceRange)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            this.maxPriceRange = maxPriceRange;
            this.selectedpriceRange = selectedPriceRange;
            this.selectedCategories = selectedCategories;
            this.productList = productList;
            InitializeComponent();
            Load();
        }

        private async void Load()
        {
            RestClient<Category> categorieRC = new RestClient<Category>();
            categories = await categorieRC.GetAsync();
            this.Content = MakeContent();
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
            foreach (var category in categories)
            {
                StackLayout categoryLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20, Padding = new Thickness(20, 0, 0, 0) };
                Switch categorySwitch = new Switch()
                {
                    ClassId = category.ID.ToString(),
                };
                categorySwitch.IsToggled = selectedCategories.Any(sc => sc.CategoryName == category.CategoryName);
                categorySwitch.Toggled += CategorySwitch_Toggled;
                categoryLayout.Children.Add(categorySwitch);
                categoryLayout.Children.Add(new Label() { Text = category.CategoryName, FontSize = 18, TextColor = Color.Black, VerticalTextAlignment = TextAlignment.Center });

                categoriesLayout.Children.Add(categoryLayout);
            }
            innerLayout.Children.Add(categoriesLayout);
            innerLayout.Children.Add(new Label() { Text = "Range de prix:", FontSize = 20, TextColor = Color.Black, FontAttributes = FontAttributes.Bold });
            RangeSlider priceRangeSlider = new RangeSlider() {MinimumValue = (float)(maxPriceRange.MinPriceRange), MaximumValue = (float)maxPriceRange.MaxPriceRange, StepValue = 0, LowerValue = (float)(selectedpriceRange.MinPriceRange), UpperValue = (float)selectedpriceRange.MaxPriceRange, ShowTextAboveThumbs = true, TextSize = 20, FormatLabel = FormaLabel};
            priceRangeSlider.DragCompleted += PriceRangeSlider_DragCompleted;
            StackLayout priceRangeLayout = new StackLayout() {Padding = new Thickness(20,0,0,0)};
            priceRangeLayout.Children.Add(priceRangeSlider);
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
            selectedpriceRange.MinPriceRange = Math.Ceiling((sender as RangeSlider).LowerValue*2)/2;
            selectedpriceRange.MaxPriceRange = Math.Ceiling((sender as RangeSlider).UpperValue*2)/2;
        }

        private string FormaLabel(Thumb thumb, float val)
        {
            return Math.Ceiling(val*2)/2 + " TND";
        }

        private async void AplyGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private void CategorySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var categorySwitch = sender as Switch;
            if (categorySwitch.IsToggled)
            {
                selectedCategories.Add(categories.FirstOrDefault(c => c.ID == Int32.Parse(categorySwitch.ClassId)));
            }
            else
            {
                selectedCategories.Remove(selectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(categorySwitch.ClassId)));
            }
        }

        protected override void OnDisappearing()
        {
            productList.AplyFilters();
        }
    }
}
