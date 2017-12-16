using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.Admin.UserViews.Orders;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.UserViews.UserListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UsersList
	{
        private readonly UserRestClient _userRc = new UserRestClient();
        private List<User> _users;
        private readonly ObservableCollection<User> _displayedUsers = new ObservableCollection<User>();

        private readonly ToolbarItem _searchToolbarItem;
        private readonly ToolbarItem _endSearchToolbarItem;


        public UsersList()
        {
            InitializeComponent();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            _displayedUsers.CollectionChanged += DisplayedUsers_CollectionChanged;
            List.ItemsSource = _displayedUsers;

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

            ToolbarItems.Add(_searchToolbarItem);
            Load();
        }

        private void DisplayedUsers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_displayedUsers == null || _displayedUsers.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = _displayedUsers.Count * 110;
                ListLayout.IsVisible = true;
            }
        }

        private async void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            var u = (User)e.Item;
            ((ListView) sender).SelectedItem = null;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var choice = await DisplayActionSheet("Choisir une action", "Annuler", null, "Consulter liste des commandes", "Editer le profile");
            switch (choice)
            {
                case "Consulter liste des commandes":
                    await Application.Current.MainPage.Navigation.PushAsync(new AuOrderList(u.ID));
                    break;
                case "Editer le profile":
                    await Application.Current.MainPage.Navigation.PushAsync(new EditProfile(u.ID, this));
                    break;
                default:
                    await PopupNavigation.PopAllAsync();
                    break;
            }
        }

        public async void Load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            try
            {
                _users = await _userRc.GetAsync();
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
            if (_users == null || _users.Count == 0)
            {
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
                return;
            }
            _users = _users.OrderBy(p => p.Name).ToList();
            _displayedUsers.Clear();
            _users.ForEach(p => _displayedUsers.Add(p));
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            if (!App.UpdateClientList) return;
            Load();
            App.UpdateClientList = false;
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
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (_users.Count == 0) return;
            ToolbarItems.Clear();
            ToolbarItems.Add(_endSearchToolbarItem);
            BodyLayout.TranslateTo(0, 0);
            SearchBar.Focus();
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AplyFilters();
        }

        private void AplyFilters()
        {
            if (_users == null || _displayedUsers == null || SearchBar == null) return;

            _users = _users.Where(u => (string.IsNullOrEmpty(SearchBar.Text) || u.Name.ToLower().StartsWith(SearchBar.Text.ToLower()))).OrderBy(p => p.Name).ToList();
            _displayedUsers.Clear();
            _users.ForEach(p => _displayedUsers.Add(p));
        }

        protected override void OnDisappearing()
        {
            ResetSearch();
        }

        private async void RemoveUser(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var text = (((((((sender as Image)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)
                ?.Parent
                as StackLayout)?.Parent as StackLayout)?.Children[0] as Label)?.Text;
            if (text == null) return;
            var id =Int32.Parse(text);
            User user;
            try
            {
                user = await _userRc.GetAsyncById(id);
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
            if (user.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Impossible de supprimer ce client, une ou plusieurs de ses commandes n'ont pas été réglées!",
                        "Ok");
                return;
            }
            if(!await DisplayAlert("Confirmation", "Êtes-vous sûr de vouloire supprimer ce client?", "Oui", "Annuler"))
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            try
            {
                if (await _userRc.DeleteAsync(user.ID))
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
            await DisplayAlert("Erreur", "Une Erreur s'est produite lors de la suppression de l'utilisateur, veuillez réessayer plus tard!.", "Ok");
        }
    }
}
