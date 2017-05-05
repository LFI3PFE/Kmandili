using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.SignIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProductList : ContentPage
	{
        private PastryShop pastryShop;
        private ToolbarItem addProduct;
        private bool toAdd = false;
        public PastryShopProductList(PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
//#pragma warning disable CS0618 // Type or member is obsolete
//            NavigationPage.SetHasNavigationBar(this, Device.OnPlatform(true, true, false));
//#pragma warning restore CS0618 // Type or member is obsolete

            addProduct = new ToolbarItem
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Icon = Device.OnPlatform(null,null,"plus.png"),
#pragma warning restore CS0618 // Type or member is obsolete
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            addProduct.Clicked += AddProduct_Clicked;

            ToolbarItems.Add(addProduct);
            load(false);
        }

        protected async override void OnDisappearing()
        {
            if(Device.RuntimePlatform == Device.Windows && !toAdd)
            {
                await Navigation.PopModalAsync();
            }
        }

        protected override void OnAppearing()
        {
            toAdd = false;
        }

        private async void RemoveProduct(object sender, EventArgs e)
        {
            int ID = Int32.Parse(((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            if (pastryShop.Products.FirstOrDefault(p => p.ID == ID).OrderProducts.All(op => op.Order.Status.StatusName == "Reçue"))
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
                RestClient<Product> productRC = new RestClient<Product>();
                await productRC.DeleteAsync(ID);
                load(true);
            }
            else
            {
                await DisplayAlert("Erreur", "Il est impossible de supprimer ce produit avant que tous ses commandes soient livrées et reçues.", "Ok");
            }
        }

        private async void AddProduct_Clicked(object sender, EventArgs e)
        {
            toAdd = true;
            await Navigation.PushAsync(new PastryShopProductForm(this, pastryShop));
        }

        public async void load(bool reload)
        {
            if (reload)
            {
                ListLayout.IsVisible = false;
                LoadingLayout.IsVisible = true;
                Loading.IsRunning = true;
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
                Loading.IsRunning = false;
                LoadingLayout.IsVisible = false;
                ListLayout.IsVisible = true;
            }
            List.ItemsSource = pastryShop.Products;
        }

        public void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}
