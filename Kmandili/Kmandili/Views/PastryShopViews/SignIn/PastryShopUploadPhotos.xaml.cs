using Kmandili.Models;
using Kmandili.Models.RestClient;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopUploadPhotos
	{
        private PastryShop _pastryShop;
        private MediaFile _mediaFileProfil;
        private MediaFile _mediaFileCover;
	    public bool ToGallery;

        public PastryShopUploadPhotos(PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
        }

        private async void ImportCover_OnClick(Object sender, EventArgs e)
        {
            ToGallery = true;
            App.GalleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileCover = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileCover == null)
                return;

            Cover.Text = _mediaFileCover.Path;
            App.GalleryIsOpent = false;
        }

        private async void ImportLogo_OnClick(Object sender, EventArgs e)
        {
            ToGallery = true;
            App.GalleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfil = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfil == null)
                return;

            Logo.Text = _mediaFileProfil.Path;
            App.GalleryIsOpent = false;
        }

        private async void NextButton_OnClick(Object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (_mediaFileProfil == null)
            {
                await DisplayAlert("Erreur", "Il faut selectionner une photo de profile!", "OK");
            }
            else if (_mediaFileCover == null)
            {
                await DisplayAlert("Erreur", "Il faut selectionner une photo de couverture!", "Ok");
            }
            else
            {
                _pastryShop.ProfilePic = await Upload(_mediaFileProfil);
                if (_pastryShop.ProfilePic == null)
                {
                    await DisplayAlert("Erreur", "Erreur pendant le téléchargement du logo!", "Ok");
                    return;
                }
                _pastryShop.CoverPic = await Upload(_mediaFileCover);
                if (_pastryShop.CoverPic == null)
                {
                    await DisplayAlert("Erreur", "Erreur pendant le téléchargement de la photo de couverture!", "Ok");
                    return;
                }
                RestClient<Address> addressRc = new RestClient<Address>();
                try
                {
                    _pastryShop.Address = await addressRc.PostAsync(_pastryShop.Address);
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
                if (_pastryShop.Address != null)
                {
                    _pastryShop.Address_FK = _pastryShop.Address.ID;
                    _pastryShop.Address = null;
                    List<PastryShopDeleveryMethod> pSdeleveryMethods =
                        new List<PastryShopDeleveryMethod>(_pastryShop.PastryShopDeleveryMethods);
                    _pastryShop.PastryShopDeleveryMethods.Clear();
                    PastryShopRestClient pastryShopRc = new PastryShopRestClient();
                    string coverPath = _pastryShop.CoverPic;
                    string logoPath = _pastryShop.ProfilePic;
                    try
                    {
                        _pastryShop = await pastryShopRc.PostAsync(_pastryShop);
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
                    
                    if (_pastryShop == null)
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
                        }
                    }
                    else
                    {
                        RestClient<PastryDeleveryPayment> pastryDeleveryPaymentRc =
                            new RestClient<PastryDeleveryPayment>();
                        foreach (PastryShopDeleveryMethod pastryShopDm in pSdeleveryMethods)
                        {
                            pastryShopDm.PastryShop_FK = _pastryShop.ID;
                            pastryShopDm.DeleveryMethod_FK = pastryShopDm.DeleveryMethod.ID;
                            pastryShopDm.DeleveryDelay_FK = pastryShopDm.DeleveryDelay.ID;
                            List<PastryDeleveryPayment> pastryDeleveryPayments =
                                pastryShopDm.PastryDeleveryPayments.ToList();
                            pastryShopDm.PastryShop = null;
                            pastryShopDm.DeleveryMethod = null;
                            pastryShopDm.DeleveryDelay = null;
                            pastryShopDm.PastryDeleveryPayments.Clear();
                            RestClient<PastryShopDeleveryMethod> pastryShopDeleveryMethodRc =
                                new RestClient<PastryShopDeleveryMethod>();
                            PastryShopDeleveryMethod psdm;
                            try
                            {

                                psdm = await pastryShopDeleveryMethodRc.PostAsync(pastryShopDm);
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
                            if (psdm == null)
                            {
                                await PopupNavigation.PopAsync();
                                return;
                            }
                            foreach (PastryDeleveryPayment p in pastryDeleveryPayments)
                            {
                                p.Payment_FK = p.Payment.ID;
                                p.Payment = null;
                                p.PastryShopDeleveryMethod = null;
                                p.PastryShopDeleveryMethod_FK = psdm.ID;
                                try
                                {
                                    if (await pastryDeleveryPaymentRc.PostAsync(p) == null)
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
                            }
                        }
                        var authorizationRestClient = new AuthorizationRestClient();
                        try
                        {
                            var tokenResponse =
                                await authorizationRestClient.AuthorizationLoginAsync(_pastryShop.Email, _pastryShop.Password);
                            if(tokenResponse == null) return;
                            Settings.SetSettings(_pastryShop.Email, _pastryShop.Password, _pastryShop.ID,
                                tokenResponse.access_token, tokenResponse.Type, tokenResponse.expires);
                            await PopupNavigation.PopAsync();
                            await Navigation.PushAsync(new PastryShopEnteringMenu(_pastryShop));
                        }
                        catch (HttpRequestException)
                        {
                            await PopupNavigation.PopAllAsync();
                            await
                                DisplayAlert("Erreur",
                                    "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                                    "Ok");
                        }
                    }
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
            catch (Exception)
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
