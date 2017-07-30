using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopSignUpForm : ContentPage
	{
        private ObservableCollection<StackLayout> PhoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private RestClient<PhoneNumberType> phoneNumberTypeRC = new RestClient<PhoneNumberType>();
        private RestClient<PriceRange> priceRangeTypeRC = new RestClient<PriceRange>();
        private List<PhoneNumberType> phoneNumberTypes;
        private List<PriceRange> priceRanges;

        public PastryShopSignUpForm()
        {
            InitializeComponent();
            PhoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            load();
        }

        private void PhoneNumberStackLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PhoneNumbersLayout.Children.Clear();
            foreach (StackLayout s in PhoneNumberStackLayouts)
            {
                s.ClassId = PhoneNumberStackLayouts.IndexOf(s).ToString();
                PhoneNumbersLayout.Children.Add(s);
            }
        }

        private async void load()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            try
            {
                phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
                priceRanges = await priceRangeTypeRC.GetAsync();
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
                return;
            }
            if (phoneNumberTypes == null || priceRanges == null) return;
            PriceRange.ItemsSource = priceRanges;
            PriceRange.SelectedIndex = 0;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
            await PopupNavigation.PopAsync();
        }

        private StackLayout CreatePhoneNumberStackLayout()
        {
            Entry phoneNumberEnry = new Entry
            {
                PlaceholderColor = Color.Gray,
                TextColor = Color.Black,
                Placeholder = "xx xxx xxx",
                WidthRequest = 150,
                FontSize = 25,
                Keyboard = Keyboard.Telephone
            };

            Picker typePicker = new Picker()
            {
                WidthRequest = 90,
                ItemsSource = phoneNumberTypes,
                SelectedIndex = 0,
            };
            Image addIcon = new Image()
            {
                Source = "add.png",
                WidthRequest = 20,
            };
            Image removeIcon = new Image()
            {
                Source = "delete.png",
                WidthRequest = 20,
                IsVisible = false
            };
            TapGestureRecognizer addPhoneNumberGR = new TapGestureRecognizer();
            addPhoneNumberGR.Tapped += AddPhoneNumber_Tapped;
            addIcon.GestureRecognizers.Add(addPhoneNumberGR);

            TapGestureRecognizer removePhoneNumberGR = new TapGestureRecognizer();
            removePhoneNumberGR.Tapped += RemovePhoneNumberGR_Tapped;
            removeIcon.GestureRecognizers.Add(removePhoneNumberGR);

            StackLayout phoneNumberStackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            phoneNumberStackLayout.Children.Add(phoneNumberEnry);
            phoneNumberStackLayout.Children.Add(typePicker);
            phoneNumberStackLayout.Children.Add(addIcon);
            phoneNumberStackLayout.Children.Add(removeIcon);

            return phoneNumberStackLayout;
        }

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((((sender as Image).Parent as StackLayout).Children[0] as Entry).Text)) return;
            (PhoneNumberStackLayouts.Last().Children[2] as Image).IsVisible = false;
            (PhoneNumberStackLayouts.Last().Children[3] as Image).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
        }

        private void RemovePhoneNumberGR_Tapped(object sender, EventArgs e)
        {
            int index = Int32.Parse(((sender as Image).Parent as StackLayout).ClassId);
            PhoneNumberStackLayouts.RemoveAt(index);
        }

        private void editorUnFocused(object sender, EventArgs e)
        {
            if (LongDesc.Text == "")
            {
                LongDesc.Text = "Longue Description...";
                LongDesc.TextColor = Color.Gray;
            }
        }

        private async Task<bool> validAddress()
        {
            int x;
            if (Number.Text == null || Number.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Numero est obligateur!", "Ok");
                return false;
            }
            else if (!int.TryParse(Number.Text, out x))
            {
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment ne doit contenir que des Chiffres!", "Ok");
                Number.Text = "";
                return false;
            }
            else if (Street.Text == null || Street.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Rue est obligateur!", "Ok");
                return false;
            }
            else if (City.Text == null || City.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Ville est obligateur!", "Ok");
                return false;
            }
            else if (ZipCode.Text == null || ZipCode.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal est obligateur!", "Ok");
                return false;
            }
            else if (ZipCode.Text.Length != 4)
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal doit contenir exactement 4 Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (!int.TryParse(ZipCode.Text, out x))
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal ne doit contenir que des Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (State.Text == null || State.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Gouvernorat est obligateur!", "Ok");
                return false;
            }
            else if (Country.Text == null || Country.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Pays est obligateur!", "Ok");
                return false;
            }
            return true;
        }

        private async Task<bool> validPhoneNumber()
        {
            int x;
            bool exist = false;
            foreach (StackLayout s in PhoneNumberStackLayouts)
            {
                string phoneNumber = (s.Children[0] as Entry).Text;
                if (phoneNumber != null && phoneNumber != "")
                {
                    if (!int.TryParse(phoneNumber, out x))
                    {
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (PhoneNumberStackLayouts.IndexOf(s) + 1) + " ne doit contenir que des chiffres!", "Ok");
                        (s.Children[0] as Entry).Text = "";
                        return false;
                    }
                    else if (phoneNumber.Length != 8)
                    {
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (PhoneNumberStackLayouts.IndexOf(s) + 1) + " doit contenir exactement 8 chiffres!", "Ok");
                        return false;
                    }
                    else
                    {
                        exist = true;
                    }
                }
            }
            if (!exist)
            {
                await DisplayAlert("Erreur", "Au moins un Numéro de Téléphone est obligatoir!", "Ok");
            }
            return exist;
        }

        private async Task<bool> valid()
        {
            if (Name.Text == null || Name.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Nom est obligateur!", "Ok");
                return false;
            }
            else if (Email.Text == null || Email.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Email est obligateur!", "Ok");
                return false;
            }
            else if (!App.isValidEmail(Email.Text))
            {
                await DisplayAlert("Erreur", "Le champ Email est invalide!", "Ok");
                Email.Text = "";
                return false;
            }
            else if (Password.Text == null || Password.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Password est obligateur!", "Ok");
                return false;
            }
            else if (!(await validAddress()))
            {
                return false;
            }
            else if (!(await validPhoneNumber()))
            {
                return false;
            }
            return true;
        }

        public async void ConfirmBt_Clicked(object sender, EventArgs e)
        {
            if (!await valid()) return;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            UserRestClient userRC = new UserRestClient();
            try
            {
                if ((await pastryShopRC.GetAsyncByEmail(Email.Text.ToLower()) != null) || (await userRC.GetAsyncByEmail(Email.Text.ToLower()) != null))
                {
                    await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                    Email.Text = "";
                    return;
                }
                await PopupNavigation.PopAllAsync();
                await PopupNavigation.PushAsync(new EmailVerificationPopupPage(this, Email.Text.ToLower()));
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur","Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
            }
        }

	    public async void EmailVerified()
	    {
            Address address = new Address()
            {
                Number = Int32.Parse(Number.Text),
                Street = Street.Text,
                City = City.Text,
                State = State.Text,
                Country = Country.Text,
                ZipCode = Int32.Parse(ZipCode.Text)
            };

            PastryShop pastryShop = new PastryShop()
            {
                Name = Name.Text.ToLower(),
                Email = Email.Text.ToLower(),
                Password = Password.Text,
                Address = address,
                LongDesc = LongDesc.Text,
                ShortDesc = ShortDesc.Text,
                PriceRange_FK = priceRanges.ElementAt(PriceRange.SelectedIndex).ID
            };
            foreach (StackLayout s in PhoneNumberStackLayouts)
            {
                if ((s.Children[0] as Entry).Text != "")
                {
                    PhoneNumber p = new PhoneNumber()
                    {
                        Number = (s.Children[0] as Entry).Text,
                        PhoneNumberType_FK = (phoneNumberTypes.ElementAt((s.Children[1] as Picker).SelectedIndex)).ID,
                    };
                    pastryShop.PhoneNumbers.Add(p);
                }
            }
            //NavigationPage navigationPage = new NavigationPage(new ContentPage());
            //await navigationPage.PushAsync(new PastryShopUploadPhotos(pastryShop));
            //toUpload = true;
            //await Navigation.PushAsync(new PastryShopUploadPhotos(pastryShop));
	        await Navigation.PushAsync(new PastryShopThirdStep(pastryShop));
	    }

        private void editorFocused(object sender, EventArgs e)
        {
            if (LongDesc.Text == "Longue Description...")
            {
                LongDesc.Text = "";
                LongDesc.TextColor = Color.Black;
            }
        }

        private void ShortDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            string _text = ShortDesc.Text;
            if (_text.Length > 30)
            {
                _text = _text.Remove(_text.Length - 1);
                ShortDesc.Text = _text;
            }
        }

        private void LongDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            string _text = LongDesc.Text;
            if (_text.Length > 100)
            {
                _text = _text.Remove(_text.Length - 1);
                LongDesc.Text = _text;
            }
        }
    }
}
