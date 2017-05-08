using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.ProductListAndFilter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductList : ContentPage
	{
        private RestClient<Product> productRC = new RestClient<Product>();
        private List<Product> products;
        private ObservableCollection<Product> displayedProducts = new ObservableCollection<Product>();
        private List<Category> selectedCategories = new List<Category>();
        private PriceRange selectedPriceRange = new PriceRange();
        private PriceRange maxPriceRange = new PriceRange();
        private SortType selectedSortType = new SortType();

        private ToolbarItem searchToolbarItem;
        private ToolbarItem endSearchToolbarItem;
        private ToolbarItem filterToolbarItem;
        private ToolbarItem sortToolbarItem;

        public ProductList ()
		{
			InitializeComponent ();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            displayedProducts.CollectionChanged += DisplayedProducts_CollectionChanged;
            List.ItemsSource = displayedProducts;

            filterToolbarItem = new ToolbarItem()
            {
                Text = "Filtrer",
                Order = ToolbarItemOrder.Primary,
                Icon = "filter.png"
            };
            filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

            searchToolbarItem = new ToolbarItem()
            {
                Text = "Chercher",
                Order = ToolbarItemOrder.Primary,
                Icon = "search.png"
            };
            searchToolbarItem.Clicked += SearchToolbarItem_Clicked;

            endSearchToolbarItem = new ToolbarItem()
            {
                Text = "Terminer",
                Order = ToolbarItemOrder.Primary,
                Icon = "close.png"
            };
            endSearchToolbarItem.Clicked += EndSearchToolbarItem_Clicked;

            sortToolbarItem = new ToolbarItem()
            {
                Text = "Trier",
                Order = ToolbarItemOrder.Primary,
                Icon = "sort.png"
            };
            sortToolbarItem.Clicked += SortToolbarItem_Clicked;

            ToolbarItems.Add(searchToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
            ToolbarItems.Add(sortToolbarItem);
            load();
		}

        private void DisplayedProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (displayedProducts == null || displayedProducts.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                ListLayout.IsVisible = true;
            }
        }

        private void SelectedNot(Object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        public async void load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            products = await productRC.GetAsync();
            if (products == null) return;
            products = products.OrderBy(p => p.Name).ToList();
            products.ForEach(p => displayedProducts.Add(p));
            selectedPriceRange.MaxPriceRange = maxPriceRange.MaxPriceRange = products.Max(p => p.Price);
            selectedPriceRange.MinPriceRange = maxPriceRange.MinPriceRange = products.Min(p => p.Price);
            selectedSortType.SortTypeIndex = 0;
            selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        private async void SortToolbarItem_Clicked(object sender, EventArgs e)
        {
            string Alph = "";
            string Rev = "";
            if (selectedSortType.SortTypeIndex == 0 && selectedSortType.IsAsc)
            {
                Alph = "Alphabet Descendant";
            }
            else
            {
                Alph = "Alphabet Ascendant";
            }
            if (selectedSortType.SortTypeIndex == 1 && selectedSortType.IsAsc)
            {
                Rev = "Prix Descendant";
            }
            else
            {
                Rev = "Prix Ascendant";
            }
            var choice = await DisplayActionSheet("Trier Par", "Annuler", null, Alph, Rev);
            if (choice == "Alphabet Descendant")
            {
                selectedSortType.SortTypeIndex = 0;
                selectedSortType.IsAsc = false;
            }
            else if (choice == "Alphabet Ascendant")
            {
                selectedSortType.SortTypeIndex = 0;
                selectedSortType.IsAsc = true;
            }
            else if (choice == "Prix Descendant")
            {
                selectedSortType.SortTypeIndex = 1;
                selectedSortType.IsAsc = false;
            }
            else if (choice == "Prix Ascendant")
            {
                selectedSortType.SortTypeIndex = 1;
                selectedSortType.IsAsc = true;
            }
            else
            {
                return;
            }
            AplyFilters();
        }

        private async void showProductMenu(Object sender, ItemTappedEventArgs e)
        {
            Product product = e.Item as Product;
            var action = await DisplayActionSheet("Choisir une action", "Annuler", null, "Ajouter au pannier", "Consulter pâtisserie");
            if (action == "Ajouter au pannier")
            {
                var index = App.Cart.FindIndex(p => p.PastryShop.ID == product.PastryShop.ID);
                if (App.Cart.Count == 0 || index < 0)
                {
                    CartPastry cartPastry = new CartPastry()
                    {
                        PastryShop = product.PastryShop,
                    };
                    CartProduct cartProduct = new CartProduct()
                    {
                        Product = product,
                        Quantity = 1
                    };
                    cartProduct.updateTotal();

                    cartPastry.CartProducts.Add(cartProduct);
                    App.Cart.Add(cartPastry);
                }else
                {
                    CartProduct cartP = App.Cart.ElementAt(index).CartProducts.FirstOrDefault(p => p.Product.ID == product.ID);
                    if (cartP == null)
                    {
                        CartProduct cartProduct = new CartProduct()
                        {
                            Product = product,
                            Quantity = 1
                        };
                        cartProduct.updateTotal();
                        App.Cart.ElementAt(index).CartProducts.Add(cartProduct);
                    }
                    else
                    {
                        cartP.Quantity++;
                        cartP.updateTotal();
                    }
                }
                await DisplayAlert("Succée", "Produit Ajouté au pannier", "OK");
            }
            else if (action == "Consulter pâtisserie")
            {
                await Navigation.PushAsync(new PastryShopProfile(product.PastryShop));
            }
        }
        
        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new FilerPopupPage(this, selectedCategories, maxPriceRange, selectedPriceRange));
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
            ToolbarItems.Add(searchToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
            ToolbarItems.Add(sortToolbarItem);
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (displayedProducts.Count != 0)
            {
                this.ToolbarItems.Clear();
                ToolbarItems.Add(endSearchToolbarItem);
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
            if(products == null || displayedProducts == null) return;
            var res =
                products.Where(
                    p =>
                        (string.IsNullOrEmpty(SearchBar.Text) || p.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) &&
                        (selectedCategories.Count == 0 || selectedCategories.Any(c => c.ID == p.Category_FK)) && 
                        (p.Price >= selectedPriceRange.MinPriceRange && p.Price <= selectedPriceRange.MaxPriceRange)).ToList();
            if (selectedSortType.SortTypeIndex == 0 && selectedSortType.IsAsc)
            {
                res = res.OrderBy(p => p.Name).ToList();
            }
            else if (selectedSortType.SortTypeIndex == 0 && !selectedSortType.IsAsc)
            {
                res = res.OrderByDescending(p => p.Name).ToList();
            }
            else if (selectedSortType.SortTypeIndex == 1 && selectedSortType.IsAsc)
            {
                res = res.OrderBy(p => p.Price).ToList();
            }
            else
            {
                res = res.OrderByDescending(p => p.Price).ToList();
            }
            displayedProducts.Clear();
            res.ForEach(p => displayedProducts.Add(p));
        }
	}
}
