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

            //Password Verification
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
                //isLoading(true);
                //NavigationPage navigationPage = new NavigationPage(new ContentPage());
                //await navigationPage.PushAsync(new UserSignUpForm());
                //isLoading(false);
                await Navigation.PushAsync(new UserSignUpForm());
            }
            else if(choice == "Pâtisserie")
            {
                //isLoading(true);
                //NavigationPage navigationPage = new NavigationPage(new ContentPage());
                //await navigationPage.PushAsync(new PastryShopSignUpForm());
                //isLoading(false);
                await Navigation.PushAsync(new PastryShopSignUpForm());
                //await Navigation.PushModalAsync(new NavigationPage(new PastryShopPointOfSaleForm(new PastryShop())));
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
                //await Navigation.PushModalAsync(new UserMasterDetailPage());
                SignInAction(Email.Text.ToLower(), Password.Text);
            }
        }

        public async void SignInAction(string email, string password)
        {
            isLoading(true);
            UserRestClient userRC = new UserRestClient();
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();

            User u = await userRC.GetAsyncByEmailAndPass(email, password);
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
                await Navigation.PushModalAsync(new UserMasterDetailPage(u));
            }
            else
            {
                PastryShop p = await pastryShopRC.GetAsyncByEmailAndPass(email, password);
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
                    await Navigation.PushModalAsync(new PastryShopMasterDetailPage(p));
                }
                else
                {
                    isLoading(false);
                    await DisplayAlert("Erreur", "Utilisateur inexistant", "OK");
                    Email.Focus();
                }
            }
        }

    }
}
