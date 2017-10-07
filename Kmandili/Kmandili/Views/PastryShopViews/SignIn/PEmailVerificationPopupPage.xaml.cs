using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.EditProfile;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PEmailVerificationPopupPage
	{
        private readonly string _email;
        private readonly PastryShopSignUpForm _pastryShopSignUpForm;
	    private readonly EditProfileInfo _editProfileInfo;
        private string _code = "";

        public PEmailVerificationPopupPage(PastryShopSignUpForm pastryShopSignUpForm, string email)
        {
            _email = email;
            _pastryShopSignUpForm = pastryShopSignUpForm;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
            SendEmail();
        }

        public PEmailVerificationPopupPage(EditProfileInfo editProfileInfo, string email)
        {
            _email = email;
            _editProfileInfo = editProfileInfo;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
            SendEmail();
        }

        private async void SendEmail()
        {
            Code.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            EmailRestClient emailRc = new EmailRestClient();
            try
            {
                _code = await emailRc.SendEmailVerification(_email);
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            Code.IsVisible = true;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Code.Text)) return;
            if (Code.Text.Length == 6 && _code == Code.Text)
            {
                if (_pastryShopSignUpForm != null)
                {
                    _pastryShopSignUpForm.EmailVerified();
                }
                else
                {
                    _editProfileInfo.EmailVerified();
                }
                await PopupNavigation.PopAllAsync();
            }
            else
            {
                Code.Text = "";
                Code.Placeholder = "Code Invalide";
                Code.PlaceholderColor = Color.Red;
            }
        }

        private async void DismissTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private void Code_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Code.Placeholder = "Code";
            Code.PlaceholderColor = Color.LightGray;
        }
    }
}
