﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmailVerificationPopupPage : PopupPage
	{
        private string email;
	    private UserSignUpForm userSignUpForm;
	    private EditUserProfile editUserProfile;
	    private string code = "";

		public EmailVerificationPopupPage (UserSignUpForm userSignUpForm, string email)
		{
		    this.email = email;
		    this.userSignUpForm = userSignUpForm;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent ();
            SendEmail();
        }

        public EmailVerificationPopupPage(EditUserProfile editUserProfile, string email)
        {
            this.email = email;
            this.editUserProfile = editUserProfile;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
            SendEmail();
        }

        private async void SendEmail()
	    {
            EmailRestClient emailRC = new EmailRestClient();
            try
            {
                code = await emailRC.SendEmailVerification(email);
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
            }
        }

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (string.IsNullOrEmpty(Code.Text)) return;
	        if (Code.Text.Length == 6 && code == Code.Text)
	        {
	            if (userSignUpForm != null)
	            {
	                userSignUpForm.EmailVerified();
	            }
	            else
	            {
                    editUserProfile.EmailVerified();
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
