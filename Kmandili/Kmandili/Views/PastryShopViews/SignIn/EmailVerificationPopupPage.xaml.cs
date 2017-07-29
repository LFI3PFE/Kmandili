using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.EditProfile;
using Kmandili.Views.UserViews;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmailVerificationPopupPage : PopupPage
	{
        private string email;
        private PastryShopSignUpForm pastryShopSignUpForm;
	    private EditProfileInfo editProfileInfo;
        private string code = "";

        public EmailVerificationPopupPage(PastryShopSignUpForm pastryShopSignUpForm, string email)
        {
            this.email = email;
            this.pastryShopSignUpForm = pastryShopSignUpForm;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
            SendEmail();
        }

        public EmailVerificationPopupPage(EditProfileInfo editProfileInfo, string email)
        {
            this.email = email;
            this.editProfileInfo = editProfileInfo;
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
            EmailRestClient emailRC = new EmailRestClient();
            code = await emailRC.SendEmailVerification(email);
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            Code.IsVisible = true;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Code.Text)) return;
            if (Code.Text.Length == 6 && code == Code.Text)
            {
                if (pastryShopSignUpForm != null)
                {
                    pastryShopSignUpForm.EmailVerified();
                }
                else
                {
                    editProfileInfo.EmailVerified();
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
