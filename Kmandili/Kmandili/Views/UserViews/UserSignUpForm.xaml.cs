﻿using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserSignUpForm : ContentPage
	{
        private ObservableCollection<StackLayout> PhoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private RestClient<PhoneNumberType> phoneNumberTypeRC = new RestClient<PhoneNumberType>();
        private List<PhoneNumberType> phoneNumberTypes;

        public UserSignUpForm ()
		{
			InitializeComponent ();
            PhoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            load();
		}

        private void PhoneNumberStackLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PhoneNumbersLayout.Children.Clear();
            foreach(StackLayout s in PhoneNumberStackLayouts)
            {
                s.ClassId = PhoneNumberStackLayouts.IndexOf(s).ToString();
                PhoneNumbersLayout.Children.Add(s);
            }
        }

        private async void load()
        {
            phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
            if (phoneNumberTypes == null) return;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
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

        private void RemovePhoneNumberGR_Tapped(object sender, EventArgs e)
        {
            int index = Int32.Parse(((sender as Image).Parent as StackLayout).ClassId);
            PhoneNumberStackLayouts.RemoveAt(index);
        }

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((((sender as Image).Parent as StackLayout).Children[0] as Entry).Text)) return;
            (PhoneNumberStackLayouts.Last().Children[2] as Image).IsVisible = false;
            (PhoneNumberStackLayouts.Last().Children[3] as Image).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
        }

        private async Task<bool> validAddress()
        {
            int x;
            if (Number.Text == null || Number.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Numero est obligateur!", "Ok");
                return false;
            } else if (!int.TryParse(Number.Text, out x))
            {
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment ne doit contenir que des Chiffres!", "Ok");
                Number.Text = "";
                return false;
            } else if (Street.Text == null || Street.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Rue est obligateur!", "Ok");
                return false;
            } else if (City.Text == null || City.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Ville est obligateur!", "Ok");
                return false;
            } else if(ZipCode.Text == null || ZipCode.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal est obligateur!", "Ok");
                return false;
            } else if(ZipCode.Text.Length != 4)
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal doit contenir exactement 4 Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            } else if (!int.TryParse(ZipCode.Text, out x))
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal ne doit contenir que des Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            } else if(State.Text == null || State.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Gouvernorat est obligateur!", "Ok");
                return false;
            } else if(Country.Text == null || Country.Text == "")
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
            foreach(StackLayout s in PhoneNumberStackLayouts)
            {
                string phoneNumber = (s.Children[0] as Entry).Text;
                if(phoneNumber != null && phoneNumber != "")
                {
                    if (!int.TryParse(phoneNumber, out x))
                    {
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (PhoneNumberStackLayouts.IndexOf(s) + 1) + " ne doit contenir que des chiffres!", "Ok");
                        (s.Children[0] as Entry).Text = "";
                        return false;
                    } else if (phoneNumber.Length != 8)
                    {
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (PhoneNumberStackLayouts.IndexOf(s) + 1) + " doit contenir exactement 8 chiffres!", "Ok");
                        return false;
                    } else
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
            }else if(LastName.Text == null || LastName.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Prenom est obligateur!", "Ok");
                return false;
            }else if(Email.Text == null || Email.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Email est obligateur!", "Ok");
                return false;
            }
            else if (!App.isValidEmail(Email.Text))
            {
                await DisplayAlert("Erreur", "Le champ Email est invalide!", "Ok");
                Email.Text = "";
                return false;
            }else if(Password.Text == null || Password.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Password est obligateur!", "Ok");
                return false;
            } else if (!(await validAddress()))
            {
                return false;
            }else if (!(await validPhoneNumber()))
            {
                return false;
            }
            return true;
        }

        public async void ConfirmBt_Clicked(object sender, EventArgs e)
        {
            if (!await valid()) return;
            UserRestClient userRC = new UserRestClient();
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            if ((await userRC.GetAsyncByEmail(Email.Text.ToLower()) != null) || (await pastryShopRC.GetAsyncByEmail(Email.Text.ToLower()) != null))
            {
                await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                Email.Text = "";
                return;
            }
            await PopupNavigation.PushAsync(new EmailVerificationPopupPage(this, Email.Text.ToLower()));
        }

	    public async void EmailVerified()
	    {
            UserRestClient userRC = new UserRestClient();
            RestClient<Address> addressRC = new RestClient<Address>();
            RestClient<PhoneNumber> phoneNumberRC = new RestClient<PhoneNumber>();
            
            Address address = new Address()
            {
                Number = Int32.Parse(Number.Text),
                Street = Street.Text,
                City = City.Text,
                State = State.Text,
                Country = Country.Text,
                ZipCode = Int32.Parse(ZipCode.Text)
            };
            address = await addressRC.PostAsync(address);
            if (address == null) return;
            User user = new User()
            {
                Name = Name.Text.ToLower(),
                LastName = LastName.Text.ToLower(),
                Email = Email.Text.ToLower(),
                Password = Password.Text,
                Address_FK = address.ID
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
                    user.PhoneNumbers.Add(p);
                }
            }
            user = await userRC.PostAsync(user);
            if (user != null)
            {
                var page = new MainPage();
                page.SignInAction(user.Email, user.Password);
                App.Current.MainPage = new NavigationPage(page);
            }
            else
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de l'enregistrement des informations!", "Ok");
                return;
            }
        }
    }
}
