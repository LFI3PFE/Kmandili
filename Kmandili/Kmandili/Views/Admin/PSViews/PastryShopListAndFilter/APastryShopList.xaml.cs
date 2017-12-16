using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kmandili.Views.Admin.PSViews.PSProfile;

namespace Kmandili.Views.Admin.PSViews.PastryShopListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class APastryShopList
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


        public APastryShopList()
        {
            InitializeComponent();
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
            }
            else if (choice == "Alphabet Ascendant")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = true;
            }
            else if (choice == "Avis Descendant")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = false;
            }
            else if (choice == "Avis Ascendant")
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
                BodyLayout.HeightRequest = _displayedPastryShops.Count * 110;
                ListLayout.IsVisible = true;
            }
        }

        private async void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            PastryShop p = (PastryShop)e.Item;
            ((ListView) sender).SelectedItem = null;
            await Application.Current.MainPage.Navigation.PushAsync(new APastryShopProfile(p, this));
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
            await PopupNavigation.PushAsync(new APastryFilterPopupPage(this, _selectedCategories));
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
            }
            else if (_selectedSortType.SortTypeIndex == 0 && !_selectedSortType.IsAsc)
            {
                res = res.OrderByDescending(p => p.Name).ToList();
            }
            else if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
            {
                res = res.OrderBy(p => p.Ratings.Sum(r => r.Value)).ToList();
            }
            else
            {
                res = res.OrderByDescending(p => p.Ratings.Sum(r => r.Value)).ToList();
            }
            _displayedPastryShops.Clear();
            res.ForEach(p => _displayedPastryShops.Add(p));
        }

        protected override void OnDisappearing()
        {
            ResetSearch();
        }

        private async void RemovePastryShop(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var pastryShopRc = new PastryShopRestClient();
            var id =
                Int32.Parse(
                    (((((((sender as Image)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent
                        as StackLayout)?.Parent as StackLayout)?.Children[0] as Label)?.Text);
            PastryShop pastryShop;
            try
            {
                pastryShop = await pastryShopRc.GetAsyncById(id);
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
            if (pastryShop.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
            {
                await
                    DisplayAlert("Erreur",
                        "Impossible de supprimer cette pâtisserie, une ou plusieurs de ses commandes n'ont pas été réglées!",
                        "Ok");
                return;
            }
            var choix = await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer cette pâtisserie?", "Oui", "Annuler");
            if (!choix) return;
            try
            {
                if (await _pastryShopRc.DeleteAsync(pastryShop.ID))
                {
                    await PopupNavigation.PopAllAsync();
                    Load();
                    return;
                }
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
            await PopupNavigation.PopAllAsync();
            await DisplayAlert("Erreur", "Une Erreur s'est produite lors de la suppression de la pâtisserie, veuillez réessayer plus tard!.", "Ok");
        }
    }
}
