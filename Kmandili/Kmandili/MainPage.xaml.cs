using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews;
using Kmandili.Views.PastryShopViews.SignIn;
using Kmandili.Views.UserViews;
using System;
using System.Linq;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Views;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Kmandili
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async Task<bool> valid()
        {
            if (Email.Text == null || Email.Text.Length == 0)
            {
                await DisplayAlert("Erreur Email", "L'adresse email est obligatoire", "OK");
                return false;
            }
            if (!App.isValidEmail(Email.Text))
            {
                await DisplayAlert("Erreur Email", "Adresse email invalide", "OK");
                return false;
            }
            
            if (Password.Text == null || Password.Text.Length == 0)
            {
                await DisplayAlert("Erreur mot de passe", "Mot de passe obligatoir", "OK");
                return false;
            }
            return true;
        }

        public async void SignUp(Object sender, EventArgs e)
        {
            var choice = await DisplayActionSheet("S'inscrire comme", "Annuler", null, "Client", "Pâtisserie");
            if (choice == "Client")
            {
                await Navigation.PushAsync(new UserSignUpForm());
            }
            else if(choice == "Pâtisserie")
            {
                await Navigation.PushAsync(new PastryShopSignUpForm());
            }
        }

        private void isLoading(bool loading)
        {
            ResetPasswordLabel.IsVisible = !loading;
            Loading.IsRunning = loading;
            Loading.IsVisible = loading;
            Email.IsEnabled = !loading;
            Password.IsEnabled = !loading;
            SignInBt.IsEnabled = !loading;
            JoinUsBt.IsEnabled = !loading;
        }

        public async void SignIn(Object sender, EventArgs e)
        {
            if (await valid())
            {
                SignInAction(Email.Text.ToLower(), Password.Text);
            }
        }

        private async void SignInAction(string email, string password)
        {
            isLoading(true);
            if (Settings.Id < 0 || App.TokenExpired())
            {
                var authorizationRestClient = new AuthorizationRestClient();
                var tokenResponse = await authorizationRestClient.AuthorizationLoginAsync(email, password);
                if (tokenResponse == null)
                {
                    isLoading(false);
                    await DisplayAlert("Erreur", "Utilisateur inexistant", "OK");
                    Email.Focus();
                    return;
                }
                Settings.SetSettings(email, password, tokenResponse.UserId, tokenResponse.access_token, tokenResponse.Type, tokenResponse.expires);
            }
            Email.Text = "";
            Password.Text = "";
            switch (Settings.Type)
            {
                case "u":
                    var userRestClient = new UserRestClient();
                    var u = await userRestClient.GetAsyncById(Settings.Id);
                    isLoading(false);
                    if (u == null)
                    {
                        await DisplayAlert("Erreur", "Impossible de se connecter au serveur.", "Ok");
                        Settings.ClearSettings();
                        return;
                    }
                    App.setMainPage(new UserMasterDetailPage(u));
                    break;
                case "p":
                    var pastryShopRestClient = new PastryShopRestClient();
                    var p = await pastryShopRestClient.GetAsyncById(Settings.Id);
                    isLoading(false);
                    if (p == null)
                    {
                        await DisplayAlert("Erreur", "Impossible de se connecter au serveur.", "Ok");
                        Settings.ClearSettings();
                        return;
                    }
                    App.setMainPage(new PastryShopMasterDetailPage(p));
                    break;
            }
        }

        private async void RestPasswordTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new PasswordResetEmailPopupPage());
        }

        protected override void OnAppearing()
        {
            if (Settings.Id > -1)
            {
                SignInAction(Settings.Email, Settings.Password);
                //App.Cart = Settings.Cart;
            }
        }
    }
}
