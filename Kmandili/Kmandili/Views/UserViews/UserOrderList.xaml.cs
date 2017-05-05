using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models.LocalModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
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

		public UserOrderList ()
		{
			InitializeComponent ();
            filterToolbarItem = new ToolbarItem()
            {
                Text = "Filtrer",
                Order = ToolbarItemOrder.Primary,
#pragma warning disable 618
                Icon = Device.OnPlatform("", "", "Filter.png"),
#pragma warning restore 618
            };
            filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

            sortToolbarItem = new ToolbarItem()
            {
                Text = "Trier",
                Order = ToolbarItemOrder.Primary,
            };
            sortToolbarItem.Clicked += SortToolbarItem_Clicked;

            ToolbarItems.Add(sortToolbarItem);
            ToolbarItems.Add(filterToolbarItem);
            OrderList.ItemsSource = displayedOrders;
            load();
		}

	    private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
	    {
	        await PopupNavigation.PushAsync(new UserOrderListFilterPopupPage(this, selectedStatuses));
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
            orders = (await orderRC.GetAsyncByUserID(App.Connected.Id)).OrderBy(o => o.SeenUser).ToList();
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
	            orders.Where(o => (selectedStatuses.Count == 0 || selectedStatuses.Any(s => s.ID == o.Status_FK))).ToList();
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
	}
}
