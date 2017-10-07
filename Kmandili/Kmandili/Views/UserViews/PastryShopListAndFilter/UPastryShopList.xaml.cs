using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Kmandili.Models.LocalModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.PastryShopListAndFilter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UPastryShopList
    {
        private readonly PastryShopRestClient _pastryShopRc = new PastryShopRestClient();
	    private List<PastryShop> _pastryShops;
	    private readonly ObservableCollection<PastryShop> _displayedPastryShops = new ObservableCollection<PastryShop>(); 
        private readonly List<Category> _selectedCategories = new List<Category>();
	    private readonly SortType _selectedSortType = new SortType();

	    private readonly ToolbarItem _searchToolbarItem;
	    private readonly ToolbarItem _endSearchToolbarItem;
	    private readonly ToolbarItem _filterToolbarItem;
	    private readonly ToolbarItem _sortToolbarItem;


        public UPastryShopList()
		{
            InitializeComponent ();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            _displayedPastryShops.CollectionChanged += DisplayedPastryShops_CollectionChanged;
            List.ItemsSource = _displayedPastryShops;
            
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
                rev = "Avis Descendant";
            }
            else
            {
                rev = "Avis Ascendant";
            }
            var choice = await DisplayActionSheet("Trier Par", "Annuler", null, alph, rev);
            if (choice == "Alphabet Descendant")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = false;
            }else if (choice == "Alphabet Ascendant")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = true;
            }else if (choice == "Avis Descendant")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = false;
            }else if (choice == "Avis Ascendant")
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

        private void DisplayedPastryShops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_displayedPastryShops == null || _displayedPastryShops.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = _displayedPastryShops.Count*110;
                ListLayout.IsVisible = true;
            }
        }

        private void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            PastryShop p = (PastryShop)e.Item;
            ((ListView) sender).SelectedItem = null;
            Navigation.PushAsync(new UPastryShopProfile(p));
        }

        public async void Load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            try
            {
                _pastryShops = await _pastryShopRc.GetAsync();
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
            if (_pastryShops == null || _pastryShops.Count == 0)
            {
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
                return;
            }
            _pastryShops = _pastryShops.OrderBy(p => p.Name).ToList();
            _displayedPastryShops.Clear();
            _pastryShops.ForEach(p => _displayedPastryShops.Add(p));
            _selectedSortType.SortTypeIndex = 0;
            _selectedSortType.IsAsc = true; 
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            if (App.UpdatePastryList)
            {
                Load();
                App.UpdatePastryList = false;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new UPastryFilterPopupPage(this, _selectedCategories));
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
            if (_pastryShops.Count != 0)
            {
                ToolbarItems.Clear();
                ToolbarItems.Add(_endSearchToolbarItem);
                BodyLayout.TranslateTo(0, 0);
                SearchBar.Focus();
            }
        }

	    private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
            AplyFilters();
        }

	    public void AplyFilters()
	    {
	        if (_pastryShops == null || _displayedPastryShops == null) return;

	        var res =
	            _pastryShops.Where(
	                p =>
	                    (string.IsNullOrEmpty(SearchBar.Text) || p.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) &&
	                    (_selectedCategories.Count == 0 ||
	                     p.Categories.Any(c => _selectedCategories.Any(sc => sc.CategoryName == c.CategoryName)))).ToList();
	        if (_selectedSortType.SortTypeIndex == 0 && _selectedSortType.IsAsc)
	        {
	            res = res.OrderBy(p => p.Name).ToList();
	        }else if (_selectedSortType.SortTypeIndex == 0 && !_selectedSortType.IsAsc)
	        {
	            res = res.OrderByDescending(p => p.Name).ToList();
	        }else if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
	        {
	            res = res.OrderBy(p => p.Ratings.Sum(r => r.Value)).ToList();
	        }
	        else
	        {
	            res = res.OrderByDescending(p => p.Ratings.Sum(r => r.Value)).ToList();
	        }
            _displayedPastryShops.Clear();
            res.ForEach(p=> _displayedPastryShops.Add(p));
	    }
        
	    protected override void OnDisappearing()
	    {
            ResetSearch();
	    }
	}
}
