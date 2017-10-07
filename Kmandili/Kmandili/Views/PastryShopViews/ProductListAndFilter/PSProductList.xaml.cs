using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.SignIn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PsProductList
	{
        private PastryShop _pastryShop;
	    private readonly ObservableCollection<Product> _displayedProducts = new ObservableCollection<Product>(); 
        private readonly List<Category> _selectedCategories = new List<Category>();
        private readonly PriceRange _selectedPriceRange = new PriceRange();
        private readonly PriceRange _maxPriceRange = new PriceRange();
        private readonly SortType _selectedSortType = new SortType();

        private readonly ToolbarItem _addProduct;
	    private readonly ToolbarItem _filterToolbarItem;
	    private readonly ToolbarItem _sortToolbarItem;
	    private readonly ToolbarItem _searchToolbarItem;
	    private readonly ToolbarItem _endSearchToolbarItem;

        public PsProductList(PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            
            _addProduct = new ToolbarItem
            {
                Icon = "plus.png",
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            _addProduct.Clicked += AddProduct_Clicked;

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

            ToolbarItems.Add(_addProduct);
            ToolbarItems.Add(_searchToolbarItem);
            ToolbarItems.Add(_sortToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);

            _displayedProducts.CollectionChanged += DisplayedProducts_CollectionChanged;
            List.ItemsSource = _displayedProducts;
            Load(false);
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
                BodyLayout.HeightRequest = _displayedProducts.Count*110;
                ListLayout.IsVisible = true;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
	    {
            await PopupNavigation.PushAsync(new PProductFilterPopupPage(this, _selectedCategories, _maxPriceRange, _selectedPriceRange));
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

        private void EndSearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private async void ResetSearch()
        {
            SearchBar.Text = "";
            SearchBar.Unfocus();
            ToolbarItems.Clear();
            ToolbarItems.Add(_addProduct);
            ToolbarItems.Add(_searchToolbarItem);
            ToolbarItems.Add(_sortToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchBar_OnTextChanged(object sender, EventArgs e)
        {
            AplyFilters();
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

        private async void RemoveProduct(object sender, EventArgs e)
        {
            if (_pastryShop.Products.Count == 1)
            {
                await DisplayAlert("Erreur", "Il faut avoir au moins un produit!", "Ok");
                return;
            }
            int id = Int32.Parse(((((((((sender as Image)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Children[0] as Label)?.Text);
            if (_pastryShop.Products.FirstOrDefault(p => p.ID == id).OrderProducts.All(op => op.Order.Status.StatusName == "Reçue"))
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
                RestClient<Product> productRc = new RestClient<Product>();
                try
                {
                    if (!(await productRc.DeleteAsync(id))) return;
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
                Load(true);
            }
            else
            {
                await DisplayAlert("Erreur", "Il est impossible de supprimer ce produit avant que tous ses commandes soient livrées et reçues.", "Ok");
            }
        }

        private async void AddProduct_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopProductForm(this, _pastryShop));
        }

        public async void Load(bool reload)
        {
            if (reload)
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
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
                if (_pastryShop == null) return;
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
            }
            _displayedProducts.Clear();
            _pastryShop.Products.OrderBy(p => p.Name).ToList().ForEach(p => _displayedProducts.Add(p));
            _selectedSortType.SortTypeIndex = 0;
            _selectedSortType.IsAsc = true;
            _selectedPriceRange.MaxPriceRange = _maxPriceRange.MaxPriceRange = (float)Math.Ceiling(_pastryShop.Products.Max(p => p.Price) * 2) / 2;
            _selectedPriceRange.MinPriceRange = _maxPriceRange.MinPriceRange = (float)Math.Floor(_pastryShop.Products.Min(p => p.Price) * 2) / 2;
        }

        public void SelectedNot(object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }

	    public void AplyFilters()
	    {
            if (_pastryShop.Products == null || _displayedProducts == null) return;
            var res =
                _pastryShop.Products.Where(
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

	    protected override void OnDisappearing()
	    {
            ResetSearch();
	    }

	    private async void ToProductDetails(object sender, ItemTappedEventArgs e)
	    {
	        var product = (Product)e.Item;
	        await Navigation.PushAsync(new ProductDetail(product, this));
	    }

	    public void Reload()
	    {
	        Load(true);
        }
    }
}
