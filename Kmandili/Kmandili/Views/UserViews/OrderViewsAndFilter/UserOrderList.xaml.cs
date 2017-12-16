using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kmandili.Helpers;
using Kmandili.Models.LocalModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOrderList
	{
	    private List<Order> _orders; 
        private readonly ObservableCollection<Order> _displayedOrders = new ObservableCollection<Order>(); 
	    private readonly List<Status> _selectedStatuses = new List<Status>();
        private readonly SortType _selectedSortType = new SortType();

	    private readonly ToolbarItem _filterToolbarItem;
	    private readonly ToolbarItem _sortToolbarItem;
	    private readonly ToolbarItem _searchToolbarItem;
	    private readonly ToolbarItem _endSearchToolbarItem;

		public UserOrderList ()
		{
			InitializeComponent ();
            OrderList.SeparatorVisibility = SeparatorVisibility.None;
		    BodyLayout.TranslateTo(0, -50);
            
            _filterToolbarItem = new ToolbarItem()
            {
                Text = "Filtrer",
                Order = ToolbarItemOrder.Primary,
                Icon = "filter.png",
            };
            _filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

            _sortToolbarItem = new ToolbarItem()
            {
                Text = "Trier",
                Order = ToolbarItemOrder.Primary,
                Icon = "sort.png"
            };
            _sortToolbarItem.Clicked += SortToolbarItem_Clicked;

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
                Icon = "close.png",
            };
            _endSearchToolbarItem.Clicked += EndSearchToolbarItem_Clicked;

            ToolbarItems.Add(_searchToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);
            ToolbarItems.Add(_sortToolbarItem);

            _displayedOrders.CollectionChanged += DisplayedProducts_CollectionChanged;
            OrderList.ItemsSource = _displayedOrders;
            Load();
		}

        private void DisplayedProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_displayedOrders == null || _displayedOrders.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                OrderList.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = _displayedOrders.Count*110;
                OrderList.IsVisible = true;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
	    {
	        await PopupNavigation.PushAsync(new UOrderFilterPopupPage(this, _selectedStatuses));
	    }

	    private async void SortToolbarItem_Clicked(object sender, EventArgs e)
	    {
            string date;
            string seen;
            if (_selectedSortType.SortTypeIndex == 0 && _selectedSortType.IsAsc)
            {
                date = "Date Descendante";
            }
            else
            {
                date = "Date Ascendante";
            }
            if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
            {
                seen = "Non vus en dernier";
            }
            else
            {
                seen = "Non vus en premier";
            }
            var choice = await DisplayActionSheet("Trier Par", "Annuler", null, date, seen);
            if (choice == "Date Descendante")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = false;
            }
            else if (choice == "Date Ascendante")
            {
                _selectedSortType.SortTypeIndex = 0;
                _selectedSortType.IsAsc = true;
            }
            else if (choice == "Non vus en premier")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = true;
            }
            else if (choice == "Non vus en dernier")
            {
                _selectedSortType.SortTypeIndex = 1;
                _selectedSortType.IsAsc = false;
            }
            else
            {
                return;
            }
            AplyFilters();
        }

        public async void Load()
        {
            OrderList.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            OrderRestClient orderRc = new OrderRestClient();
            try
            {
                _orders = await orderRc.GetAsyncByUserId(Settings.Id);
            }
            catch (Exception)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                await Navigation.PopAsync();
                return;
            }
            if (_orders == null) return;
            _orders = _orders.OrderBy(o => o.SeenUser).ToList();
            _displayedOrders.Clear();
            _orders.ForEach(o => _displayedOrders.Add(o));
            _selectedSortType.SortTypeIndex = 1;
            _selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            OrderList.IsVisible = true;
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }

        private async void ToOrderDetail(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new UserOrderDetail(this, e.Item as Order));
        }

	    public void AplyFilters()
	    {
            if (_orders == null || _displayedOrders == null) return;
	        var res =
	            _orders.Where(o => (string.IsNullOrEmpty(SearchBar.Text) || o.PastryShop.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) && (_selectedStatuses.Count == 0 || _selectedStatuses.Any(s => s.ID == o.Status_FK))).ToList();
            if (_selectedSortType.SortTypeIndex == 0 && _selectedSortType.IsAsc)
            {
                res = res.OrderBy(o => o.Date).ToList();
            }
            else if (_selectedSortType.SortTypeIndex == 0 && !_selectedSortType.IsAsc)
            {
                res = res.OrderByDescending(o => o.Date).ToList();
            }
            else if (_selectedSortType.SortTypeIndex == 1 && _selectedSortType.IsAsc)
            {
                res = res.OrderBy(o => o.SeenUser).ToList();
            }
            else
            {
                res = res.OrderByDescending(o => o.SeenUser).ToList();
            }
            _displayedOrders.Clear();
            res.ForEach(p => _displayedOrders.Add(p));
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
            ToolbarItems.Add(_sortToolbarItem);
            ToolbarItems.Add(_filterToolbarItem);
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (_displayedOrders.Count != 0)
            {
                this.ToolbarItems.Clear();
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
	}
}
