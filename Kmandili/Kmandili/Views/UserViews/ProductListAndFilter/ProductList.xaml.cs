using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.ProductListAndFilter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductList
    {
        private readonly RestClient<Product> _productRc = new RestClient<Product>();
        private List<Product> _products;
        private readonly ObservableCollection<Product> _displayedProducts = new ObservableCollection<Product>();
        private readonly List<Category> _selectedCategories = new List<Category>();
        private readonly PriceRange _selectedPriceRange = new PriceRange();
        private readonly PriceRange _maxPriceRange = new PriceRange();
        private readonly SortType _selectedSortType = new SortType();

        private readonly ToolbarItem _searchToolbarItem;
        private readonly ToolbarItem _endSearchToolbarItem;
        private readonly ToolbarItem _filterToolbarItem;
        private readonly ToolbarItem _sortToolbarItem;

        public ProductList ()
		{
			InitializeComponent ();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            _displayedProducts.CollectionChanged += DisplayedProducts_CollectionChanged;
            List.ItemsSource = _displayedProducts;

            _filterToolbarItem = new ToolbarItem()
            {
                Text = "Filtrer",
                Order = ToolbarItemOrder.Primary,
                Icon = "filter.png"
            };
            _filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

            _searchToolbarItem = new ToolbarItem()
            {
                Text = "Chercher",
                Order = ToolbarItemOrder.Primary,
                Icon = "search.png"
            };
            _searchToolbarItem.Clicked += SearchToolbarItem_Clicked;

            _endSearchToolbarItem = new ToolbarItem()
            {
                Text = "Terminer",
                Order = ToolbarItemOrder.Primary,
                Icon = "close.png"
            };
            _endSearchToolbarItem.Clicked += EndSearchToolbarItem_Clicked;

            _sortToolbarItem = new ToolbarItem()
            {
                Text = "Trier",
                Order = ToolbarItemOrder.Primary,
                Icon = "sort.png"
            };
            _sortToolbarItem.Clicked += SortToolbarItem_Clicked;

            ToolbarItems.Add(_searchToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);
            ToolbarItems.Add(_sortToolbarItem);
            Load();
		}

        private void DisplayedProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_displayedProducts == null || _displayedProducts.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = _displayedProducts.Count * 110;
                ListLayout.IsVisible = true;
            }
        }

        private void SelectedNot(Object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }

        public async void Load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            try
            {
                _products = await _productRc.GetAsync();
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
            if (_products == null || _products.Count == 0)
            {
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
                return;
            }
            _products = _products.OrderBy(p => p.Name).ToList();
            _products.ForEach(p => _displayedProducts.Add(p));
            _selectedPriceRange.MaxPriceRange = _maxPriceRange.MaxPriceRange = (float)Math.Ceiling(_products.Max(p => p.Price) * 2) / 2;
            _selectedPriceRange.MinPriceRange = _maxPriceRange.MinPriceRange = (float)Math.Floor(_products.Min(p => p.Price) * 2) / 2;
            _selectedSortType.SortTypeIndex = 0;
            _selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        private async void SortToolbarItem_Clicked(object sender, EventArgs e)
        {
            string alph;
            string rev;
            if (_selectedSortType.SortTypeIndex == 0 && _selectedSortType.IsAsc)
            {
                alph = "Alphabet Descendant";
            }
            else
            {
                alph = "Alphabet Ascendant";
            }
            if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
            {
                rev = "Prix Descendant";
            }
            else
            {
                rev = "Prix Ascendant";
            }
            var choice = await DisplayActionSheet("Trier Par", "Annuler", null, alph, rev);
            if (choice == "Alphabet Descendant")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = false;
            }
            else if (choice == "Alphabet Ascendant")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = true;
            }
            else if (choice == "Prix Descendant")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = false;
            }
            else if (choice == "Prix Ascendant")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = true;
            }
            else
            {
                return;
            }
            AplyFilters();
        }

        private async void ShowProductMenu(Object sender, ItemTappedEventArgs e)
        {
            Product product = e.Item as Product;
            var action = await DisplayActionSheet("Choisir une action", "Annuler", null, "Ajouter au pannier", "Consulter pâtisserie");
            if (action == "Ajouter au pannier")
            {
                var index = App.Cart.FindIndex(p => product != null && p.PastryShop.ID == product.PastryShop.ID);
                if (App.Cart.Count == 0 || index < 0)
                {
                    CartPastry cartPastry = new CartPastry()
                    {
                        PastryShop = product?.PastryShop,
                    };
                    CartProduct cartProduct = new CartProduct()
                    {
                        Product = product,
                        Quantity = 1
                    };
                    cartProduct.UpdateTotal();

                    cartPastry.CartProducts.Add(cartProduct);
                    App.Cart.Add(cartPastry);
                }else
                {
                    CartProduct cartP = App.Cart.ElementAt(index).CartProducts.FirstOrDefault(p => product != null && p.Product.ID == product.ID);
                    if (cartP == null)
                    {
                        CartProduct cartProduct = new CartProduct()
                        {
                            Product = product,
                            Quantity = 1
                        };
                        cartProduct.UpdateTotal();
                        App.Cart.ElementAt(index).CartProducts.Add(cartProduct);
                    }
                    else
                    {
                        cartP.Quantity++;
                        cartP.UpdateTotal();
                    }
                }
                await DisplayAlert("Succée", "Produit Ajouté au chariot", "OK");
            }
            else if (action == "Consulter pâtisserie")
            {
                await Navigation.PushAsync(new UPastryShopProfile(product?.PastryShop));
            }
        }
        
        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new UProductFilerPopupPage(this, _selectedCategories, _maxPriceRange, _selectedPriceRange));
        }

        private void EndSearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private async void ResetSearch()
        {
            SearchBar.Text = "";
            SearchBar.Unfocus();
            ToolbarItems.Clear();
            ToolbarItems.Add(_searchToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);
            ToolbarItems.Add(_sortToolbarItem);
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (_displayedProducts.Count != 0)
            {
                ToolbarItems.Clear();
                ToolbarItems.Add(_endSearchToolbarItem);
                BodyLayout.TranslateTo(0, 0);
                SearchBar.Focus();
            }
        }

        private void SearchBar_OnTextChanged(object sender, EventArgs e)
        {
            AplyFilters();
        }

        protected override void OnDisappearing()
        {
            ResetSearch();
        }

        public void AplyFilters()
        {
            if(_products == null || _displayedProducts == null) return;
            var res =
                _products.Where(
                    p =>
                        (string.IsNullOrEmpty(SearchBar.Text) || p.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) &&
                        (_selectedCategories.Count == 0 || _selectedCategories.Any(c => c.ID == p.Category_FK)) && 
                        (p.Price >= _selectedPriceRange.MinPriceRange && p.Price <= _selectedPriceRange.MaxPriceRange)).ToList();
            if (_selectedSortType.SortTypeIndex == 0 && _selectedSortType.IsAsc)
            {
                res = res.OrderBy(p => p.Name).ToList();
            }
            else if (_selectedSortType.SortTypeIndex == 0 && !_selectedSortType.IsAsc)
            {
                res = res.OrderByDescending(p => p.Name).ToList();
            }
            else if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
            {
                res = res.OrderBy(p => p.Price).ToList();
            }
            else
            {
                res = res.OrderByDescending(p => p.Price).ToList();
            }
            _displayedProducts.Clear();
            res.ForEach(p => _displayedProducts.Add(p));
        }
	}
}
