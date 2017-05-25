using Kmandili.Models;
using Kmandili.Models.LocalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.PSProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProductList : ContentPage
	{
        private PastryShop pastryShop;
	    private ObservableCollection<Product> displayedProducts = new ObservableCollection<Product>();
        private List<Category> selectedCategories = new List<Category>();
        private PriceRange selectedPriceRange = new PriceRange();
        private PriceRange maxPriceRange = new PriceRange();
        private SortType selectedSortType = new SortType();

        private ToolbarItem filterToolbarItem;
        private ToolbarItem sortToolbarItem;
        private ToolbarItem searchToolbarItem;
        private ToolbarItem endSearchToolbarItem;

        public PastryShopProductList (PastryShop pastryShop)
		{
			InitializeComponent ();
            this.pastryShop = pastryShop;
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            
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

            displayedProducts.CollectionChanged += DisplayedProducts_CollectionChanged;
            List.ItemsSource = displayedProducts;
            pastryShop.Products.OrderBy(p => p.Name).ToList().ForEach(p => displayedProducts.Add(p));
            selectedSortType.SortTypeIndex = 0;
            selectedSortType.IsAsc = true;
            selectedPriceRange.MaxPriceRange = maxPriceRange.MaxPriceRange = pastryShop.Products.Max(p => p.Price);
            selectedPriceRange.MinPriceRange = maxPriceRange.MinPriceRange = pastryShop.Products.Min(p => p.Price);
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

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new FilterPopupPage(this, selectedCategories, maxPriceRange, selectedPriceRange));
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

        private void SearchBar_OnTextChanged(object sender, EventArgs e)
        {
            AplyFilters();
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

        public void AplyFilters()
        {
            if (pastryShop.Products == null || displayedProducts == null) return;
            var res =
                pastryShop.Products.Where(
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

        protected override void OnDisappearing()
        {
            ResetSearch();
        }

        public void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        public async void addToCart(object sender, EventArgs e)
        {
            int id = Int32.Parse((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            Product product = pastryShop.Products.FirstOrDefault(p => p.ID == id);
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
            }
            else
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
    }
}
