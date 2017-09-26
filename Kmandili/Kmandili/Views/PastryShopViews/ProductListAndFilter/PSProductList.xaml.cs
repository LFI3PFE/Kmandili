using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.SignIn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PSProductList : ContentPage
	{
        private PastryShop pastryShop;
	    private ObservableCollection<Product> displayedProducts = new ObservableCollection<Product>(); 
        private List<Category> selectedCategories = new List<Category>();
        private PriceRange selectedPriceRange = new PriceRange();
        private PriceRange maxPriceRange = new PriceRange();
        private SortType selectedSortType = new SortType();

        private ToolbarItem addProduct;
	    private ToolbarItem filterToolbarItem;
	    private ToolbarItem sortToolbarItem;
	    private ToolbarItem searchToolbarItem;
	    private ToolbarItem endSearchToolbarItem;

        public PSProductList(PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            
            addProduct = new ToolbarItem
            {
                Icon = "plus.png",
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            addProduct.Clicked += AddProduct_Clicked;

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

            ToolbarItems.Add(addProduct);
            ToolbarItems.Add(searchToolbarItem);
            ToolbarItems.Add(sortToolbarItem);
            ToolbarItems.Add(filterToolbarItem);

            displayedProducts.CollectionChanged += DisplayedProducts_CollectionChanged;
            List.ItemsSource = displayedProducts;
            Load(false);
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
                BodyLayout.HeightRequest = (double) (displayedProducts.Count*110);
                ListLayout.IsVisible = true;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
	    {
            await PopupNavigation.PushAsync(new PProductFilterPopupPage(this, selectedCategories, maxPriceRange, selectedPriceRange));
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
            ToolbarItems.Add(addProduct);
            ToolbarItems.Add(searchToolbarItem);
            ToolbarItems.Add(sortToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
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

        private async void RemoveProduct(object sender, EventArgs e)
        {
            if (pastryShop.Products.Count == 1)
            {
                await DisplayAlert("Erreur", "Il faut avoir au moins un produit!", "Ok");
                return;
            }
            int ID = Int32.Parse(((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            if (pastryShop.Products.FirstOrDefault(p => p.ID == ID).OrderProducts.All(op => op.Order.Status.StatusName == "Reçue"))
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
                RestClient<Product> productRC = new RestClient<Product>();
                try
                {
                    if (!(await productRC.DeleteAsync(ID))) return;
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
            await Navigation.PushAsync(new PastryShopProductForm(this, pastryShop));
        }

        public async void Load(bool reload)
        {
            if (reload)
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                try
                {
                    pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
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
                if (pastryShop == null) return;
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
            }
            displayedProducts.Clear();
            pastryShop.Products.OrderBy(p => p.Name).ToList().ForEach(p => displayedProducts.Add(p));
            selectedSortType.SortTypeIndex = 0;
            selectedSortType.IsAsc = true;
            selectedPriceRange.MaxPriceRange = maxPriceRange.MaxPriceRange = (float)Math.Ceiling(pastryShop.Products.Max(p => p.Price) * 2) / 2;
            selectedPriceRange.MinPriceRange = maxPriceRange.MinPriceRange = (float)Math.Floor(pastryShop.Products.Min(p => p.Price) * 2) / 2;
        }

        public void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
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
