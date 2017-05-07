using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using Kmandili.Views.PastryShopViews.POSListAndAdd;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.ProductListAndFilter;

namespace Kmandili.Views.PastryShopViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProfile : ContentPage
	{
        private ToolbarItem ProductList;
        private ToolbarItem pointOfSaleList;
        private PastryShop pastryShop;
        private PastryShopMasterDetailPage pastryShopMasterDetailPage;

        public PastryShopProfile(PastryShopMasterDetailPage pastryShopMasterDetailPage, PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
            //ToolbarItems.Clear();
            ProductList = new ToolbarItem
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Icon = Device.OnPlatform(null, null, "products.png"),
#pragma warning restore CS0618 // Type or member is obsolete
                Text = "Produits",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            ProductList.Clicked += ProductListOnClick;

            pointOfSaleList = new ToolbarItem()
            {
                Text = "Points de vente",
                Order = ToolbarItemOrder.Secondary
            };
            pointOfSaleList.Clicked += PointOfSaleList_Clicked;

            ToolbarItems.Add(ProductList);
            ToolbarItems.Add(pointOfSaleList);
            RefreshRating();
            load();
        }

        private async void PointOfSaleList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PointOfSalesList(pastryShop));
        }

        public async void Reload()
        {
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            load();
        }

        protected override void OnAppearing()
        {
            pastryShopMasterDetailPage.IsGestureEnabled = true;
        }

        protected override void OnDisappearing()
        {
            pastryShopMasterDetailPage.IsGestureEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void load()
        {
            Cover.Source = pastryShop.CoverPic;
            ProfilImage.Source = pastryShop.ProfilePic;
            PastryName.Text = pastryShop.Name;
            Address.Text = pastryShop.Address.ToString();
            Desc.Text = pastryShop.LongDesc;
            Email.Text = pastryShop.Email;
            PhoneNumbersLayout.Children.Clear();
            foreach (PhoneNumber phone in pastryShop.PhoneNumbers)
            {
                PhoneNumbersLayout.Children.Add(new Label() { Text = phone.Number + " " + phone.PhoneNumberType.Type, TextColor = Color.Black, FontSize = 20 });
            }
            //CoreStackLayout.Children.Clear();
            //foreach (PointOfSale pointOfSale in pastryShop.PointOfSales)
            //{
            //    CoreStackLayout.Children.Add(MakePointOfSaleStackLayout(pointOfSale));
            //}
        }

        public async void RefreshRating()
        {
            PastryShopRestClient rs = new PastryShopRestClient();
            PastryShop pp = await rs.GetAsyncById(pastryShop.ID);
            pastryShop = pp;
            Rating.Text = pastryShop.Rating.ToString();
            NumberOfReviews.Text = "(" + pastryShop.NumberOfRatings.ToString() + " avis)";
        }

        public async void ProductListOnClick(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PSProductList(pastryShop));
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.WinPhone:
            //        await Navigation.PushModalAsync(new NavigationPage(new PastryShopProductList(pastryShop)));
            //        break;
            //    case Device.Windows:
            //        NavigationPage nav = new NavigationPage(new ContentPage());
            //        await nav.PushAsync(new PastryShopProductList(pastryShop));
            //        await Navigation.PushModalAsync(nav);
            //        break;
            //    default:
            //        await Navigation.PushAsync(new PastryShopProductList(pastryShop));
            //        break;
            //}
        }

    }
}
