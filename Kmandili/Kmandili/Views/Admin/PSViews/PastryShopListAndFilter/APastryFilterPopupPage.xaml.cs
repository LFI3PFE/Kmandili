using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Kmandili.Models;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.PSViews.PastryShopListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class APastryFilterPopupPage
	{
        private readonly List<Category> _selectedCategories;
        private List<Category> _categories;
        private readonly APastryShopList _pastryShopList;
        public APastryFilterPopupPage(APastryShopList pastryShopList, List<Category> selectedCategories)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            _selectedCategories = selectedCategories;
            _pastryShopList = pastryShopList;
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
            Label aplyLabel = new Label() { Text = "Appliquer", TextColor = Color.DodgerBlue, FontSize = 20, HorizontalOptions = LayoutOptions.End };
            TapGestureRecognizer aplyGestureRecognizer = new TapGestureRecognizer();
            aplyGestureRecognizer.Tapped += AplyGestureRecognizer_Tapped;
            aplyLabel.GestureRecognizers.Add(aplyGestureRecognizer);
            innerLayout.Children.Add(aplyLabel);
            mainLayout.Children.Add(innerLayout);

            return mainLayout;
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
            _pastryShopList.AplyFilters();
        }
    }
}
