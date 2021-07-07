using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopUploadPhotos : ContentPage
	{
        private PastryShop pastryShop;
        private MediaFile _mediaFileProfil;
        private MediaFile _mediaFileCover;
        private bool toGallery = false;

        public PastryShopUploadPhotos(PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
        }

        private async void ImportCover_OnClick(Object sender, EventArgs e)
        {
            toGallery = true;
            App.galleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileCover = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileCover == null)
                return;

            Cover.Text = _mediaFileCover.Path;
            App.galleryIsOpent = false;
        }

        private async void ImportLogo_OnClick(Object sender, EventArgs e)
        {
            toGallery = true;
            App.galleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfil = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfil == null)
                return;

            Logo.Text = _mediaFileProfil.Path;
            App.galleryIsOpent = false;
        }

        private async void NextButton_OnClick(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (_mediaFileProfil == null)
            {
                await DisplayAlert("Erreur", "Il faut selectionner une photo de profile!", "OK");
                return;
            }
            else if (_mediaFileCover == null)
            {
                await DisplayAlert("Erreur", "Il faut selectionner une photo de couverture!", "Ok");
                return;
            }
            else
            {
                pastryShop.ProfilePic = await Upload(_mediaFileProfil);
                pastryShop.CoverPic = await Upload(_mediaFileCover);

                await PopupNavigation.PopAsync();
                await Navigation.PushAsync(new PastryShopThirdStep(pastryShop));
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
    }
}
