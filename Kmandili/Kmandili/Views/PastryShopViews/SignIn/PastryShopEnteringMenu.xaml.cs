﻿using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopEnteringMenu : ContentPage
	{
        private PastryShop pastryShop;
        public PastryShopEnteringMenu(PastryShop pastryShop)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            this.pastryShop = pastryShop;
            load();
        }

        public async void load()
        {
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            if (pastryShop == null) return;
            if(pastryShop.Products.Count == 0)
            {
                NoResultsLabel.IsVisible = true;
            }
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            Liste.ItemsSource = pastryShop.Products;
        }

        public async void DeleteProduct(Object sender, EventArgs e)
        {
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            int ID = Int32.Parse(((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            RestClient<Product> productRC = new RestClient<Product>();
            if(!(await productRC.DeleteAsync(ID))) return;
            load();
        }

        public async void AddProduct_OnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopProductForm(this, pastryShop));
        }

        public async void Continue(Object sender, EventArgs e)
        {
            if (pastryShop.Products.Count != 0)
            {
                await Navigation.PushAsync(new PastryShopEnteringPointOfSales(pastryShop));
            }
            else
            {
                await DisplayAlert("Erreur", "Au moins une produit doit être entré!", "Ok");
            }
        }

        public void selected(Object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
