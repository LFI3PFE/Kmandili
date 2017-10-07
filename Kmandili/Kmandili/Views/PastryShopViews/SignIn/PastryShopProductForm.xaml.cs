using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Views.PastryShopViews.ProductListAndFilter;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProductForm
	{
        private readonly PastryShop _pastryShop;
        private List<Category> _categories;
        private List<SaleUnit> _saleUnits;
        private MediaFile _mediaFileProfil;
	    public bool ToGallery;
        private readonly PastryShopEnteringMenu _productsPage;
        private readonly PsProductList _productsList;

        public PastryShopProductForm(PastryShopEnteringMenu productsPage, PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            _productsPage = productsPage;
            Load();
        }

        public PastryShopProductForm(PsProductList productsList, PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            _productsList = productsList;
            Load();
        }

        public async void Load()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            RestClient<SaleUnit> saleUnitRc = new RestClient<SaleUnit>();
            try
            {
                _saleUnits = await saleUnitRc.GetAsync();
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
            if (_saleUnits == null) return;
            PickerUnit.ItemsSource = _saleUnits;
            PickerUnit.SelectedIndex = 0;

            RestClient<Category> categoryRc = new RestClient<Category>();
            try
            {
                _categories = await categoryRc.GetAsync();
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
            if (_categories == null) return;
            CategoryPicker.ItemsSource = _categories;
            CategoryPicker.SelectedIndex = 0;
            await PopupNavigation.PopAsync();
        }

        public async Task<bool> ValidForm()
        {
            if (string.IsNullOrEmpty(ProductName.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Nam est obligatoir!", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(Price.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Prix est obligatoir!", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(ProductDescription.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ description est obligatoir!", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(ProductPhoto.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une photo du produit est obligatoire!", "Ok");
                return false;
            }
            return true;
        }

        public async void NextButton_OnClick(Object seneder, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (await ValidForm())
            {
                RestClient<Product> productRc = new RestClient<Product>();
                Product product = new Product()
                {
                    Name = ProductName.Text,
                    Price = double.Parse(Price.Text),
                    Description = ProductDescription.Text,
                    SaleUnit_FK = _saleUnits.ElementAt(PickerUnit.SelectedIndex).ID,
                    Category_FK = _categories.ElementAt(CategoryPicker.SelectedIndex).ID,
                    PastryShop_FK = _pastryShop.ID,
                    Pic = await Upload(_mediaFileProfil)
                };

                try
                {
                    product = await productRc.PostAsync(product);
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
                if (product == null)
                {
                    await PopupNavigation.PopAsync();
                    await DisplayAlert("Erreur", "Erreur dans l'ajout du produit!", "Ok");
                }
                else
                {
                    if(_productsPage != null)
                    {
                        _productsPage.Load();
                    }
                    else
                    {
                        _productsList.Load(true);
                    }
                    await PopupNavigation.PopAsync();
                    await Navigation.PopAsync();
                }
            }
        }

        private async Task<string> Upload(MediaFile upfile)
        {
            string fileName = Guid.NewGuid().ToString();
            var stream = upfile.GetStream();
            try
            {
                var res = await new UploadRestClient().Upload(stream, fileName);
                if (res)
                {
                    return App.ServerUrl + "Uploads/" + fileName + ".jpg";
                }
                return null;
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return null;
            }
        }

        public async void ImportImg_OnClick(Object seneder, EventArgs e)
        {
            ToGallery = true;
            App.GalleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfil = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfil == null)
                return;

            ProductPhoto.Text = _mediaFileProfil.Path;
            ToGallery = false;
            App.GalleryIsOpent = false;
        }
    }
}
