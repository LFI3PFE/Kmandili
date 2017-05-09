using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews;
using Kmandili.Views.PastryShopViews.SignIn;
using Kmandili.Views.UserViews;
using System;
using System.Threading.Tasks;
using Kmandili.Views;
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
                Connected connected = new Connected();
                connected.type = typeof(User).Name;
                connected.Id = u.ID;
                connected.Email = u.Email;
                connected.Password = u.Password;

                App.Connected = connected;
                isLoading(false);
                Email.Text = "";
                Password.Text = "";
                //App.Current.MainPage = new UserMasterDetailPage(u);
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        App.Current.MainPage = new UserMasterDetailPage(u);
                        break;
                    case Device.Android:
                        App.Current.MainPage = new NavigationPage(new UserMasterDetailPage(u));
                        break;
                    case Device.WinPhone:
                    case Device.Windows:
                        App.Current.MainPage = new NavigationPage(new UserMasterDetailPage(u));
                        break;
                    default:
                        App.Current.MainPage = new NavigationPage(new UserMasterDetailPage(u));
                        break;
                }
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
                    Connected connected = new Connected();
                    connected.type = typeof(PastryShop).Name;
                    connected.Id = p.ID;
                    connected.Email = p.Email;
                    connected.Password = p.Password;

                    App.Connected = connected;
                    isLoading(false);
                    Email.Text = "";
                    Password.Text = "";
                    //App.Current.MainPage = new PastryShopMasterDetailPage(p);
                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            App.Current.MainPage = new PastryShopMasterDetailPage(p);
                            break;
                        case Device.Android:
                            App.Current.MainPage = new NavigationPage(new PastryShopMasterDetailPage(p));
                            break;
                        case Device.WinPhone:
                        case Device.Windows:
                            App.Current.MainPage = new NavigationPage(new PastryShopMasterDetailPage(p));
                            break;
                        default:
                            App.Current.MainPage = new NavigationPage(new PastryShopMasterDetailPage(p));
                            break;
                    }
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
    }
}
