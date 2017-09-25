﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
	public partial class UProductFilerPopupPage : PopupPage
	{
        private List<Category> selectedCategories;
        private List<Category> categories;
        private ProductList productList;
	    private PriceRange selectedpriceRange;
	    private PriceRange maxPriceRange;
        public UProductFilerPopupPage(ProductList productList, List<Category> selectedCategories, PriceRange maxPriceRange, PriceRange selectedPriceRange)
        {

            InitializeComponent();
            BackgroundColor = Color.FromHex("#CC000000");
            this.maxPriceRange = maxPriceRange;
            this.selectedpriceRange = selectedPriceRange;
            this.selectedCategories = selectedCategories;
            this.productList = productList;
            Load();
        }

        private async void Load()
        {
            RestClient<Category> categorieRC = new RestClient<Category>();
            try
            {
                categories = await categorieRC.GetAsync();
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
            if (categories == null) return;
            this.Content = MakeContent();
        }

        private StackLayout MakeContent()
        {
            StackLayout mainLayout = new StackLayout()
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
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
            innerLayout.Children.Add(new Label() { Text = "Plage de prix:", FontSize = 20, TextColor = Color.Black, FontAttributes = FontAttributes.Bold });
            StackLayout priceRangeLayout = new StackLayout() { Padding = new Thickness(20, 0, 0, 0) };
            if (maxPriceRange.MinPriceRange != maxPriceRange.MaxPriceRange)
            {
                RangeSlider priceRangeSlider = new RangeSlider() { MinimumValue = (float)maxPriceRange.MinPriceRange, MaximumValue = (float)maxPriceRange.MaxPriceRange, StepValue = 0.00f, LowerValue = (float)selectedpriceRange.MinPriceRange, UpperValue = (float)selectedpriceRange.MaxPriceRange, ShowTextAboveThumbs = true, TextSize = 20, FormatLabel = FormaLabel };
                priceRangeSlider.DragCompleted += PriceRangeSlider_DragCompleted;
                priceRangeLayout.Children.Add(priceRangeSlider);
            }
            else
            {
                priceRangeLayout.Children.Add(new Label() { Text = "Un seul prix: " + maxPriceRange.MinPriceRange + "dt", HorizontalTextAlignment = TextAlignment.Center });
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