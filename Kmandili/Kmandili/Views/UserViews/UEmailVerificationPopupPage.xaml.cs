using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UEmailVerificationPopupPage
	{
        private readonly string _email;
	    private readonly UserSignUpForm _userSignUpForm;
	    private readonly EditUserProfile _editUserProfile;
	    private string _code = "";

		public UEmailVerificationPopupPage(UserSignUpForm userSignUpForm, string email)
		{
		    _email = email;
		    _userSignUpForm = userSignUpForm;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent ();
            SendEmail();
        }

        public UEmailVerificationPopupPage(EditUserProfile editUserProfile, string email)
        {
            _email = email;
            _editUserProfile = editUserProfile;
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            InitializeComponent();
            SendEmail();
        }

        private async void SendEmail()
	    {
            EmailRestClient emailRc = new EmailRestClient();
            try
            {
                _code = await emailRc.SendEmailVerification(_email);
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
	        if (Code.Text.Length == 6 && _code == Code.Text)
	        {
	            if (_userSignUpForm != null)
	            {
	                _userSignUpForm.EmailVerified();
	            }
	            else
	            {
                    _editUserProfile.EmailVerified();
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
