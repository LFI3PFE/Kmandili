﻿using System;
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
            SendEmail();
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
        }

        public EmailVerificationPopupPage(EditProfileInfo editProfileInfo, string email)
        {
            this.email = email;
            this.editProfileInfo = editProfileInfo;
            SendEmail();
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
        }

        private async void SendEmail()
        {
            EmailRestClient emailRC = new EmailRestClient();
            code = await emailRC.SendEmailVerification(email);
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
                await PopupNavigation.PopAsync();
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
