﻿using System;
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
	public partial class UsersList : ContentPage
	{
        private UserRestClient userRC = new UserRestClient();
        private List<User> users;
        private ObservableCollection<User> displayedUsers = new ObservableCollection<User>();
        //private List<Category> selectedCategories = new List<Category>();
        //private SortType selectedSortType = new SortType();

        private ToolbarItem searchToolbarItem;
        private ToolbarItem endSearchToolbarItem;
        //private ToolbarItem filterToolbarItem;
        //private ToolbarItem sortToolbarItem;


        public UsersList()
        {
            InitializeComponent();
            BodyLayout.TranslateTo(0, -50);
            List.SeparatorVisibility = SeparatorVisibility.None;
            displayedUsers.CollectionChanged += DisplayedUsers_CollectionChanged;
            List.ItemsSource = displayedUsers;

            //filterToolbarItem = new ToolbarItem()
            //{
            //    Text = "Filtrer",
            //    Order = ToolbarItemOrder.Primary,
            //    Icon = "filter.png"
            //};
            //filterToolbarItem.Clicked += FilterToolbarItem_Clicked;

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

            //sortToolbarItem = new ToolbarItem()
            //{
            //    Text = "Trier",
            //    Order = ToolbarItemOrder.Primary,
            //    Icon = "sort.png"
            //};
            //sortToolbarItem.Clicked += SortToolbarItem_Clicked;

            ToolbarItems.Add(searchToolbarItem);
            //ToolbarItems.Add(filterToolbarItem);
            //ToolbarItems.Add(sortToolbarItem);
            load();
        }

        //private async void SortToolbarItem_Clicked(object sender, EventArgs e)
        //{
        //    string Alph = "";
        //    string Rev = "";
        //    if (selectedSortType.SortTypeIndex == 0 && selectedSortType.IsAsc)
        //    {
        //        Alph = "Alphabet Descendant";
        //    }
        //    else
        //    {
        //        Alph = "Alphabet Ascendant";
        //    }
        //    if (selectedSortType.SortTypeIndex == 1 && selectedSortType.IsAsc)
        //    {
        //        Rev = "Avis Descendant";
        //    }
        //    else
        //    {
        //        Rev = "Avis Ascendant";
        //    }
        //    var choice = await DisplayActionSheet("Trier Par", "Annuler", null, Alph, Rev);
        //    if (choice == "Alphabet Descendant")
        //    {
        //        selectedSortType.SortTypeIndex = 0;
        //        selectedSortType.IsAsc = false;
        //    }
        //    else if (choice == "Alphabet Ascendant")
        //    {
        //        selectedSortType.SortTypeIndex = 0;
        //        selectedSortType.IsAsc = true;
        //    }
        //    else if (choice == "Avis Descendant")
        //    {
        //        selectedSortType.SortTypeIndex = 1;
        //        selectedSortType.IsAsc = false;
        //    }
        //    else if (choice == "Avis Ascendant")
        //    {
        //        selectedSortType.SortTypeIndex = 1;
        //        selectedSortType.IsAsc = true;
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    AplyFilters();
        //}

        private void DisplayedUsers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (displayedUsers == null || displayedUsers.Count == 0)
            {
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
            }
            else
            {
                EmptyLabel.IsVisible = false;
                BodyLayout.HeightRequest = (double)(displayedUsers.Count * 110);
                ListLayout.IsVisible = true;
            }
        }

        private async void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            var u = (User)e.Item;
            (sender as ListView).SelectedItem = null;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var choice = await DisplayActionSheet("Choisir une action", "Annuler", null, "Consulter liste des commandes", "Editer le profile");
            switch (choice)
            {
                case "Consulter liste des commandes":
                    await App.Current.MainPage.Navigation.PushAsync(new AUOrderList(u.ID));
                    break;
                case "Editer le profile":
                    await App.Current.MainPage.Navigation.PushAsync(new EditProfile(u.ID, this));
                    break;
                default:
                    await PopupNavigation.PopAllAsync();
                    break;
            }
        }

        public async void load()
        {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            try
            {
                users = await userRC.GetAsync();
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
            if (users == null || users.Count == 0)
            {
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                EmptyLabel.IsVisible = true;
                ListLayout.IsVisible = false;
                return;
            }
            users = users.OrderBy(p => p.Name).ToList();
            displayedUsers.Clear();
            users.ForEach(p => displayedUsers.Add(p));
            //selectedSortType.SortTypeIndex = 0;
            //selectedSortType.IsAsc = true;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            ListLayout.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            if (App.updateClientList)
            {
                load();
                App.updateClientList = false;
            }
        }

        //private async void FilterToolbarItem_Clicked(object sender, EventArgs e)
        //{
        //    //await PopupNavigation.PushAsync(new FilterPopupPage(this, selectedCategories));
        //}

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
            await BodyLayout.TranslateTo(0, -50);
        }

        private void SearchToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (users.Count != 0)
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

        private void AplyFilters()
        {
            if (users == null || displayedUsers == null || SearchBar == null) return;

            users = users.Where(u => (string.IsNullOrEmpty(SearchBar.Text) || u.Name.ToLower().StartsWith(SearchBar.Text.ToLower()))).OrderBy(p => p.Name).ToList();
            displayedUsers.Clear();
            users.ForEach(p => displayedUsers.Add(p));
        }

        protected override void OnDisappearing()
        {
            ResetSearch();
        }

        private async void RemoveUser(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var id =
                Int32.Parse(
                    (((((((sender as Image)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent
                        as StackLayout)?.Parent as StackLayout)?.Children[0] as Label)?.Text);
            User user;
            try
            {
                user = await userRC.GetAsyncById(id);
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
                if (await userRC.DeleteAsync(user.ID))
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
            await DisplayAlert("Erreur", "Une Erreur s'est produite lors de la suppression de l'utilisateur, veuillez réessayer plus tard!.", "Ok");
        }
    }
}
