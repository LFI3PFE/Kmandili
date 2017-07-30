using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
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

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public async void load()
        {
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            AddBt.IsEnabled = false;
            ContinueBt.IsEnabled = false;
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            try
            {
                pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
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
            if (pastryShop == null) return;
            ContinueBt.IsEnabled = true;
            if (pastryShop.Products.Count == 0)
            {
                NoResultsLabel.IsVisible = true;
                ContinueBt.IsEnabled = false;
            }
            AddBt.IsEnabled = true;
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            Liste.ItemsSource = pastryShop.Products;
        }

        public async void DeleteProduct(Object sender, EventArgs e)
        {
            Label label =
                ((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as
                    StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label);
            int ID = Int32.Parse(label.Text);
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            NoResultsLabel.IsVisible = false;
            Liste.ItemsSource = null;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            RestClient<Product> productRC = new RestClient<Product>();
            try
            {
                if (!(await productRC.DeleteAsync(ID)))
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
