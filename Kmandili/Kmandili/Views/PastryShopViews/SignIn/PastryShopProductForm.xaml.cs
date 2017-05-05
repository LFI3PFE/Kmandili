using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProductForm : ContentPage
	{
        private PastryShop pastryShop;
        private List<Category> categories;
        private List<SaleUnit> saleUnits;
        private MediaFile _mediaFileProfil;
        private bool toGallery = false;
        private PastryShopEnteringMenu productsPage;
        private PastryShopProductList ProductsList;

        public PastryShopProductForm(PastryShopEnteringMenu ProductsPage, PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.productsPage = ProductsPage;
            Load();
        }

        public PastryShopProductForm(PastryShopProductList ProductsList, PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.ProductsList = ProductsList;
            Load();
        }

        public async void Load()
        {
            RestClient<SaleUnit> saleUnitRC = new RestClient<SaleUnit>();
            PickerUnit.ItemsSource = saleUnits = await saleUnitRC.GetAsync();
            PickerUnit.SelectedIndex = 0;

            RestClient<Category> categoryRC = new RestClient<Category>();
            CategoryPicker.ItemsSource = categories = await categoryRC.GetAsync();
            CategoryPicker.SelectedIndex = 0;
        }

        public async Task<bool> ValidForm()
        {
            if (ProductName.Text == null || ProductName.Text.Length == 0)
            {
                await DisplayAlert("Erreur", "Le champ Nam est obligatoir!", "Ok");
                return false;
            }

            if (Price.Text == null || Price.Text.Length == 0)
            {
                await DisplayAlert("Erreur", "Le champ Prix est obligatoir!", "Ok");
                return false;
            }

            if (ProductDescription.Text == null || ProductDescription.Text.Length == 0)
            {
                await DisplayAlert("Erreur", "Le champ description est obligatoir!", "Ok");
                return false;
            }

            if (ProductPhoto.Text == null || ProductPhoto.Text.Length == 0)
            {
                await DisplayAlert("Erreur", "Une photo du produit est obligatoire!", "Ok");
                return false;
            }
            return true;
        }

        public async void NextButton_OnClick(Object seneder, EventArgs e)
        {
            if (await ValidForm())
            {
                RestClient<Product> productRC = new RestClient<Product>();
                Product product = new Product()
                {
                    Name = ProductName.Text,
                    Price = double.Parse(Price.Text),
                    Description = ProductDescription.Text,
                    SaleUnit_FK = saleUnits.ElementAt(PickerUnit.SelectedIndex).ID,
                    Category_FK = categories.ElementAt(CategoryPicker.SelectedIndex).ID,
                    PastryShop_FK = pastryShop.ID,
                    Pic = await Upload(_mediaFileProfil)
                };

                product = await productRC.PostAsync(product);
                if (product == null)
                {
                    await DisplayAlert("Erreur", "Erreur dans l'ajout du produit!", "Ok");
                    return;
                }
                else
                {
                    if(productsPage != null)
                    {
                        productsPage.load();
                    }
                    else
                    {
                        ProductsList.load(true);
                    }
                    await Navigation.PopAsync();
                }
            }
        }

        private async Task<string> Upload(MediaFile upfile)
        {
            string fileName = Guid.NewGuid().ToString();
            var stream = upfile.GetStream();
            var res = await new UploadRestClient().Upload(stream, fileName);
            if (res)
            {
                return App.ServerURL + "Uploads/" + fileName + ".jpg";
            }
            return null;
        }

        public async void ImportImg_OnClick(Object seneder, EventArgs e)
        {
            toGallery = true;
            App.galleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfil = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfil == null)
                return;

            ProductPhoto.Text = _mediaFileProfil.Path;
            toGallery = false;
            App.galleryIsOpent = false;
        }

        public void EntryFocused(Object sender, EventArgs e)
        {

        }
    }
}
