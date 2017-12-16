using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AEmailVerificationPopupPage
	{
        private readonly string _email;
        private readonly EditAdmin _editAdmin;
        private string _code = "";

        public AEmailVerificationPopupPage(EditAdmin editAdmin, string email)
        {
            _email = email;
            _editAdmin = editAdmin;
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
                _editAdmin.EmailVerified();
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
