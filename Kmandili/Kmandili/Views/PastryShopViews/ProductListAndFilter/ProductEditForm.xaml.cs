using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductEditForm
	{
	    private Product _product;
	    private List<SaleUnit> _saleUnits;
	    private List<Category> _categories;
        private MediaFile _mediaFilePic;
	    private readonly ProductDetail _productDetail;

	    private bool _updateParent;

        public ProductEditForm (Product product, ProductDetail productDetail)
		{
			InitializeComponent ();
		    _product = product;
            _productDetail = productDetail;
            Load(false);
		}

	    private async void Load(bool reload)
	    {
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        if (reload)
	        {
	            var productRc = new RestClient<Product>();
	            try
	            {
                    _product = await productRc.GetAsyncById(_product.ID);
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
	            if (_product == null)
	            {
	                await DisplayAlert("Erreur", "Erreur lors de la récupération des données.", "Ok");
	                await PopupNavigation.PopAsync();
	                return;
	            }
	        }
	        var saleUnitRc = new RestClient<SaleUnit>();
	        var categorieRc = new RestClient<Category>();
	        try
	        {
                _saleUnits = await saleUnitRc.GetAsync();
                _categories = await categorieRc.GetAsync();
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
            if (_saleUnits == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            if (_categories == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            ProductName.Text = _product.Name;
	        Price.Text = _product.Price.ToString(CultureInfo.InvariantCulture);
	        PicPreview.Source = _product.Pic;
            ProductDescription.Text = _product.Description;
	        PickerUnit.ItemsSource = _saleUnits;
	        PickerUnit.SelectedIndex = _saleUnits.IndexOf(_saleUnits.FirstOrDefault(s => s.ID == _product.SaleUnit_FK));
	        CategoryPicker.ItemsSource = _categories;
	        CategoryPicker.SelectedIndex = _categories.IndexOf(_categories.FirstOrDefault(s => s.ID == _product.Category_FK));
	        await PopupNavigation.PopAsync();
	    }

	    private async void Changer_OnClick(object sender, EventArgs e)
	    {
            await CrossMedia.Current.Initialize();

            _mediaFilePic = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFilePic == null)
                return;

            PicPreview.Source = _mediaFilePic.Path;
        }

        private async void NextButton_OnClick(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var picSrc = _product.Pic;
            if (_mediaFilePic != null)
            {
                if (!(await Delete(_product.Pic)))
                {
                    await PopupNavigation.PopAsync();
                    await DisplayAlert("Erreur", "Erreur lors de la mise à jour du produit.", "Ok");
                    return;
                }
                picSrc = await Upload(_mediaFilePic);
                if (picSrc == null)
                {
                    await PopupNavigation.PopAsync();
                    await DisplayAlert("Erreur", "Erreur lors de la mise à jour du produit.", "Ok");
                    return;
                }
            }
            Product newProduct = new Product()
            {
                ID = _product.ID,
                Name = ProductName.Text,
                Description = ProductDescription.Text,
                SaleUnit_FK = ((SaleUnit) PickerUnit.SelectedItem).ID,
                Category_FK = ((Category) CategoryPicker.SelectedItem).ID,
                PastryShop_FK = _product.PastryShop_FK,
                Pic = picSrc,
                Price = double.Parse(Price.Text),
            };
            var productRc = new RestClient<Product>();
            try
            {
                if (!(await productRc.PutAsync(_product.ID, newProduct)))
                {
                    await PopupNavigation.PopAsync();
                    await DisplayAlert("Erreur", "Erreur lors de la mise à jour du produit.", "Ok");
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
            _updateParent = true;
            await Navigation.PopAsync();
        }

	    protected override void OnDisappearing()
	    {
	        if (_updateParent)
	        {
	            _productDetail.Reload();
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

        private async Task<bool> Delete(string picUrl)
        {
            string fileName = picUrl.Substring(App.ServerUrl.Length + 8, (picUrl.Length - (App.ServerUrl.Length + 8)));
            fileName = fileName.Substring(0, (fileName.Length - 4));
            UploadRestClient uploadRc = new UploadRestClient();
            try
            {
                return (await uploadRc.Delete(fileName));
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return false;
            }
        }

    }
}
