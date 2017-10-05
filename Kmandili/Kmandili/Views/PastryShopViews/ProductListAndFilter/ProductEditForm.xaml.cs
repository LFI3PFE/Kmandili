using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductEditForm : ContentPage
	{
	    private Product product;
	    private List<SaleUnit> saleUnits;
	    private List<Category> categories;
        private MediaFile _mediaFilePic;
	    private ProductDetail productDetail;

	    private bool updateParent = false;

        public ProductEditForm (Product product, ProductDetail productDetail)
		{
			InitializeComponent ();
		    this.product = product;
            this.productDetail = productDetail;
            Load(false);
		}

	    private async void Load(bool reload)
	    {
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        if (reload)
	        {
	            var productRC = new RestClient<Product>();
	            try
	            {
                    this.product = await productRC.GetAsyncById(product.ID);
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
	            if (this.product == null)
	            {
	                await DisplayAlert("Erreur", "Erreur lors de la récupération des données.", "Ok");
	                await PopupNavigation.PopAsync();
	                return;
	            }
	        }
	        var saleUnitRC = new RestClient<SaleUnit>();
	        var categorieRC = new RestClient<Category>();
	        try
	        {
                saleUnits = await saleUnitRC.GetAsync();
                categories = await categorieRC.GetAsync();
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
            if (saleUnits == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            if (categories == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            ProductName.Text = product.Name;
	        Price.Text = product.Price.ToString();
	        PicPreview.Source = product.Pic;
            ProductDescription.Text = product.Description;
	        PickerUnit.ItemsSource = saleUnits;
	        PickerUnit.SelectedIndex = saleUnits.IndexOf(saleUnits.FirstOrDefault(s => s.ID == product.SaleUnit_FK));
	        CategoryPicker.ItemsSource = categories;
	        CategoryPicker.SelectedIndex = categories.IndexOf(categories.FirstOrDefault(s => s.ID == product.Category_FK));
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
            var picSrc = product.Pic;
            if (_mediaFilePic != null)
            {
                if (!(await Delete(product.Pic)))
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
                ID = product.ID,
                Name = ProductName.Text,
                Description = ProductDescription.Text,
                SaleUnit_FK = (PickerUnit.SelectedItem as SaleUnit).ID,
                Category_FK = (CategoryPicker.SelectedItem as Category).ID,
                PastryShop_FK = product.PastryShop_FK,
                Pic = picSrc,
                Price = double.Parse(Price.Text),
            };
            var productRC = new RestClient<Product>();
            try
            {
                if (!(await productRC.PutAsync(product.ID, newProduct)))
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
            updateParent = true;
            await Navigation.PopAsync();
        }

	    protected override void OnDisappearing()
	    {
	        if (updateParent)
	        {
	            productDetail.Reload();
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
                    return App.ServerURL + "Uploads/" + fileName + ".jpg";
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

        private async Task<bool> Delete(string picURL)
        {
            string fileName = picURL.Substring(App.ServerURL.Count() + 8, (picURL.Length - (App.ServerURL.Count() + 8)));
            fileName = fileName.Substring(0, (fileName.Length - 4));
            UploadRestClient uploadRC = new UploadRestClient();
            try
            {
                return (await uploadRC.Delete(fileName));
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
