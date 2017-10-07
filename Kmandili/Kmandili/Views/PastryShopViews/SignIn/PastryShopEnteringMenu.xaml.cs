﻿using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopEnteringMenu
    {
        private PastryShop _pastryShop;
        public PastryShopEnteringMenu(PastryShop pastryShop)
        {
            InitializeComponent();
            Liste.SeparatorVisibility = SeparatorVisibility.None;
            NavigationPage.SetHasBackButton(this, false);
            _pastryShop = pastryShop;
            Load();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public async void Load()
        {
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            AddBt.IsEnabled = false;
            ContinueBt.IsEnabled = false;
            PastryShopRestClient pastryShopRc = new PastryShopRestClient();
            try
            {
                _pastryShop = await pastryShopRc.GetAsyncById(_pastryShop.ID);
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
            if (_pastryShop == null) return;
            ContinueBt.IsEnabled = true;
            if (_pastryShop.Products.Count == 0)
            {
                NoResultsLabel.IsVisible = true;
                ContinueBt.IsEnabled = false;
            }
            AddBt.IsEnabled = true;
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            Liste.ItemsSource = _pastryShop.Products;
        }

        public async void DeleteProduct(Object sender, EventArgs e)
        {
            Label label =
                ((((((((sender as Image)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.Parent as
                    StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Children[0] as Label);
            int id = Int32.Parse(label?.Text);
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            RestClient<Product> productRc = new RestClient<Product>();
            try
            {
                if (!(await productRc.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAsync();
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
                await Navigation.PopAsync();
                return;
            }
            await PopupNavigation.PopAsync();
            Load();
        }

        public async void AddProduct_OnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PastryShopProductForm(this, _pastryShop));
        }

        public async void Continue(Object sender, EventArgs e)
        {
            if (_pastryShop.Products.Count != 0)
            {
                await Navigation.PushAsync(new PastryShopEnteringPointOfSales(_pastryShop));
            }
            else
            {
                await DisplayAlert("Erreur", "Au moins une produit doit être entré!", "Ok");
            }
        }

        public void Selected(Object sender, EventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}
