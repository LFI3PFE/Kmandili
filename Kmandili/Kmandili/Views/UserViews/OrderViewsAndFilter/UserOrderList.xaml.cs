using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models.LocalModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews.OrderViewsAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOrderList : ContentPage
	{
	    private List<Order> orders; 
        private ObservableCollection<Order> displayedOrders = new ObservableCollection<Order>(); 
	    private List<Status> selectedStatuses = new List<Status>();
        private SortType selectedSortType = new SortType();

	    private ToolbarItem filterToolbarItem;
	    private ToolbarItem sortToolbarItem;
	    private ToolbarItem searchToolbarItem;
	    private ToolbarItem endSearchToolbarItem;

		public UserOrderList ()
		{
			InitializeComponent ();
		    BodyLayout.TranslateTo(0, -50);
            
            filterToolbarItem = new ToolbarItem()
            {
                Text = "Filtrer",
                Order = ToolbarItemOrder.Primary,
                Icon = "filter.png",
            };
            filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

            sortToolbarItem = new ToolbarItem()
            {
                Text = "Trier",
                Order = ToolbarItemOrder.Primary,
                Icon = "sort.png"
            };
            sortToolbarItem.Clicked += SortToolbarItem_Clicked;

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
                Icon = "close.png",
            };
            endSearchToolbarItem.Clicked += EndSearchToolbarItem_Clicked;

            ToolbarItems.Add(searchToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
            ToolbarItems.Add(sortToolbarItem);

            displayedOrders.CollectionChanged += DisplayedProducts_CollectionChanged;
            OrderList.ItemsSource = displayedOrders;
            load();
		}

        private void DisplayedProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (displayedOrders == null || displayedOrders.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                OrderList.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = (double) (displayedOrders.Count*110);
                OrderList.IsVisible = true;
            }
        }

        private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
	    {
	        await PopupNavigation.PushAsync(new FilterPopupPage(this, selectedStatuses));
	    }

	    private async void SortToolbarItem_Clicked(object sender, EventArgs e)
	    {
            string Date = "";
            string Seen = "";
            if (selectedSortType.SortTypeIndex == 0 && selectedSortType.IsAsc)
            {
                Date = "Date Descendante";
            }
            else
            {
                Date = "Date Ascendante";
            }
            if (selectedSortType.SortTypeIndex == 1 && selectedSortType.IsAsc)
            {
                Seen = "Non vus en dernier";
            }
            else
            {
                Seen = "Non vus en premier";
            }
            var choice = await DisplayActionSheet("Trier Par", "Annuler", null, Date, Seen);
            if (choice == "Date Descendante")
            {
                selectedSortType.SortTypeIndex = 0;
                selectedSortType.IsAsc = false;
            }
            else if (choice == "Date Ascendante")
            {
                selectedSortType.SortTypeIndex = 0;
                selectedSortType.IsAsc = true;
            }
            else if (choice == "Non vus en premier")
            {
                selectedSortType.SortTypeIndex = 1;
                selectedSortType.IsAsc = true;
            }
            else if (choice == "Non vus en dernier")
            {
                selectedSortType.SortTypeIndex = 1;
                selectedSortType.IsAsc = false;
            }
            else
            {
                return;
            }
            AplyFilters();
        }

        public async void load()
        {
            OrderList.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            OrderRestClient orderRC = new OrderRestClient();
            orders = await orderRC.GetAsyncByUserID(Settings.Id);
            if (orders == null) return;
            orders = orders.OrderBy(o => o.SeenUser).ToList();
            displayedOrders.Clear();
            orders.ForEach(o => displayedOrders.Add(o));
            selectedSortType.SortTypeIndex = 1;
            selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            OrderList.IsVisible = true;
        }

        private void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private async void ToOrderDetail(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new UserOrderDetail(this, e.Item as Order));
        }

	    public void AplyFilters()
	    {
            if (orders == null || displayedOrders == null) return;
	        var res =
	            orders.Where(o => (string.IsNullOrEmpty(SearchBar.Text) || o.PastryShop.Name.ToLower().StartsWith(SearchBar.Text.ToLower())) && (selectedStatuses.Count == 0 || selectedStatuses.Any(s => s.ID == o.Status_FK))).ToList();
            if (selectedSortType.SortTypeIndex == 0 && selectedSortType.IsAsc)
            {
                res = res.OrderBy(o => o.Date).ToList();
            }
            else if (selectedSortType.SortTypeIndex == 0 && !selectedSortType.IsAsc)
            {
                res = res.OrderByDescending(o => o.Date).ToList();
            }
            else if (selectedSortType.SortTypeIndex == 1 && selectedSortType.IsAsc)
            {
                res = res.OrderBy(o => o.SeenUser).ToList();
            }
            else
            {
                res = res.OrderByDescending(o => o.SeenUser).ToList();
            }
            displayedOrders.Clear();
            res.ForEach(p => displayedOrders.Add(p));
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
            ToolbarItems.Add(sortToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (displayedOrders.Count != 0)
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
	}
}
