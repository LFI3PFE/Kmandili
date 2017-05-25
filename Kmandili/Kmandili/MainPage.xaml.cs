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

        public async void SignInAction(string email, string password)
        {
            isLoading(true);
            UserRestClient userRC = new UserRestClient();
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            User u;
            try
            {
                u = await userRC.GetAsyncByEmailAndPass(email, password);
            }
            catch (ConnectionLostException e)
            {
                isLoading(false);
                return;
            }
            if (u != null)
            {
                if (Settings.Id < 0)
                {
                    Settings.Email = u.Email;
                    Settings.Password = u.Password;
                    Settings.Id = u.ID;
                    Settings.Token = "";
                    isLoading(false);
                }
                Email.Text = "";
                Password.Text = "";
                App.setMainPage(new UserMasterDetailPage(u));
            }
            else
            {
                PastryShop p;
                try
                {
                    p = await pastryShopRC.GetAsyncByEmailAndPass(email, password);
                }
                catch (ConnectionLostException e)
                {
                    isLoading(false);
                    return;
                }
                if (p != null)
                {
                    if (Settings.Id < 0)
                    {
                        Settings.Email = p.Email;
                        Settings.Password = p.Password;
                        Settings.Id = p.ID;
                        Settings.Token = "";
                    }
                    isLoading(false);
                    Email.Text = "";
                    Password.Text = "";
                    App.setMainPage(new PastryShopMasterDetailPage(p));
                }
                else
                {
                    isLoading(false);
                    await DisplayAlert("Erreur", "Utilisateur inexistant", "OK");
                    Email.Focus();
                }
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
