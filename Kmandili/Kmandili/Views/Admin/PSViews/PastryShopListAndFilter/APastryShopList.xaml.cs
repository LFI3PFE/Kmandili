using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
	public partial class APastryShopList : ContentPage
	{
        private PastryShopRestClient pastryShopRC = new PastryShopRestClient();
        private List<PastryShop> pastryShops;
        private ObservableCollection<PastryShop> displayedPastryShops = new ObservableCollection<PastryShop>();
        private List<Category> selectedCategories = new List<Category>();
        private SortType selectedSortType = new SortType();

        private ToolbarItem searchToolbarItem;
        private ToolbarItem endSearchToolbarItem;
        private ToolbarItem filterToolbarItem;
        private ToolbarItem sortToolbarItem;


        public APastryShopList()
        {
            InitializeComponent();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            displayedPastryShops.CollectionChanged += DisplayedPastryShops_CollectionChanged;
            List.ItemsSource = displayedPastryShops;

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
                Rev = "Avis Descendant";
            }
            else
            {
                Rev = "Avis Ascendant";
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
            else if (choice == "Avis Descendant")
            {
                selectedSortType.SortTypeIndex = 1;
                selectedSortType.IsAsc = false;
            }
            else if (choice == "Avis Ascendant")
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

        private void DisplayedPastryShops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (displayedPastryShops == null || displayedPastryShops.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = (double)(displayedPastryShops.Count * 110);
                ListLayout.IsVisible = true;
            }
        }

        private async void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            PastryShop p = (PastryShop)e.Item;
            (sender as ListView).SelectedItem = null;
            await App.Current.MainPage.Navigation.PushAsync(new APastryShopProfile(p, this));
        }

        public async void load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            try
            {
                pastryShops = await pastryShopRC.GetAsync();
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
            if (pastryShops == null || pastryShops.Count == 0)
            {
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
                return;
            }
            pastryShops = pastryShops.OrderBy(p => p.Name).ToList();
            displayedPastryShops.Clear();
            pastryShops.ForEach(p => displayedPastryShops.Add(p));
            selectedSortType.SortTypeIndex = 0;
            selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            if (App.updatePastryList)
            {
                load();
                App.updatePastryList = false;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new APastryFilterPopupPage(this, selectedCategories));
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
            if (pastryShops.Count != 0)
            {
                this.ToolbarItems.Clear();
                ToolbarItems.Add(endSearchToolbarItem);
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
            if (pastryShops == null || displayedPastryShops == null) return;

            var res =
                pastryShops.Where(
                    p =>
                        (string.IsNullOrEmpty(SearchBar.Text) || p.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) &&
                        (selectedCategories.Count == 0 ||
                         p.Categories.Any(c => selectedCategories.Any(sc => sc.CategoryName == c.CategoryName)))).ToList();
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
                res = res.OrderBy(p => p.Ratings.Sum(r => r.Value)).ToList();
            }
            else
            {
                res = res.OrderByDescending(p => p.Ratings.Sum(r => r.Value)).ToList();
            }
            displayedPastryShops.Clear();
            res.ForEach(p => displayedPastryShops.Add(p));
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
                if (await pastryShopRC.DeleteAsync(pastryShop.ID))
                {
                    await PopupNavigation.PopAllAsync();
                    load();
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
