using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
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
                if (pastryShop.ProfilePic == null)
                {
                    await DisplayAlert("Erreur", "Erreur pendant le téléchargement du logo!", "Ok");
                    return;
                }
                pastryShop.CoverPic = await Upload(_mediaFileCover);
                if (pastryShop.CoverPic == null)
                {
                    await DisplayAlert("Erreur", "Erreur pendant le téléchargement de la photo de couverture!", "Ok");
                    return;
                }
                RestClient<Address> addressRC = new RestClient<Address>();
                pastryShop.Address = await addressRC.PostAsync(pastryShop.Address);
                if (pastryShop.Address != null)
                {
                    pastryShop.Address_FK = pastryShop.Address.ID;
                    pastryShop.Address = null;
                    List<PastryShopDeleveryMethod> PSdeleveryMethods =
                        new List<PastryShopDeleveryMethod>(pastryShop.PastryShopDeleveryMethods);
                    pastryShop.PastryShopDeleveryMethods.Clear();
                    PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                    string coverPath = pastryShop.CoverPic;
                    string logoPath = pastryShop.ProfilePic;
                    pastryShop = await pastryShopRC.PostAsync(pastryShop);
                    
                    if (pastryShop == null)
                    {
                        await PopupNavigation.PopAsync();
                        await DisplayAlert("Erreur", "Erreur lors de l'enregistrement des informations!", "Ok");
                        if (!(await Delete(coverPath)))
                        {
                            await DisplayAlert("Erreur", "Erreur lors de la supression de la couverture!", "Ok");
                            return;
                        }
                        if (!(await Delete(logoPath)))
                        {
                            await DisplayAlert("Erreur", "Erreur lors de la supression de la couverture!", "Ok");
                            return;
                        }
                        return;
                    }
                    else
                    {
                        RestClient<PastryDeleveryPayment> pastryDeleveryPaymentRC =
                            new RestClient<PastryDeleveryPayment>();
                        foreach (PastryShopDeleveryMethod pastryShopDM in PSdeleveryMethods)
                        {
                            pastryShopDM.PastryShop_FK = pastryShop.ID;
                            pastryShopDM.DeleveryMethod_FK = pastryShopDM.DeleveryMethod.ID;
                            pastryShopDM.DeleveryDelay_FK = pastryShopDM.DeleveryDelay.ID;
                            List<PastryDeleveryPayment> pastryDeleveryPayments =
                                pastryShopDM.PastryDeleveryPayments.ToList();
                            pastryShopDM.PastryShop = null;
                            pastryShopDM.DeleveryMethod = null;
                            pastryShopDM.DeleveryDelay = null;
                            pastryShopDM.PastryDeleveryPayments.Clear();
                            RestClient<PastryShopDeleveryMethod> pastryShopDeleveryMethodRC =
                                new RestClient<PastryShopDeleveryMethod>();
                            PastryShopDeleveryMethod PSDM = await pastryShopDeleveryMethodRC.PostAsync(pastryShopDM);
                            if (PSDM == null)
                            {
                                await PopupNavigation.PopAsync();
                                return;
                            }
                            foreach (PastryDeleveryPayment p in pastryDeleveryPayments)
                            {
                                //p.PastryShopDeleveryMethods.Add(pastryShopDM);
                                p.Payment_FK = p.Payment.ID;
                                p.Payment = null;
                                p.PastryShopDeleveryMethod = null;
                                p.PastryShopDeleveryMethod_FK = PSDM.ID;
                                if (await pastryDeleveryPaymentRC.PostAsync(p) == null)
                                {
                                    await PopupNavigation.PopAsync();
                                    return;
                                }
                            }
                        }
                        var authorizationRestClient = new AuthorizationRestClient();
                        var tokenResponse =
                            await authorizationRestClient.AuthorizationLoginAsync(pastryShop.Email, pastryShop.Password);
                        Settings.SetSettings(pastryShop.Email, pastryShop.Password, pastryShop.ID,
                            tokenResponse.access_token, tokenResponse.Type, tokenResponse.expires);
                        await PopupNavigation.PopAsync();
                        await Navigation.PushAsync(new PastryShopEnteringMenu(pastryShop));
                    }
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

        private async Task<bool> Delete(string picURL)
        {
            string fileName = picURL.Substring(App.ServerURL.Count() + 8, (picURL.Length - (App.ServerURL.Count() + 8)));
            fileName = fileName.Substring(0, (fileName.Length - 4));
            UploadRestClient uploadRC = new UploadRestClient();
            return (await uploadRC.Delete(fileName));
        }
    }
}
