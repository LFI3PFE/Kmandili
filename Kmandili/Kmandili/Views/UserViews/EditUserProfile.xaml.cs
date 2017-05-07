using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditUserProfile : ContentPage
	{
        private ObservableCollection<StackLayout> PhoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private RestClient<PhoneNumberType> phoneNumberTypeRC = new RestClient<PhoneNumberType>();
        private List<PhoneNumberType> phoneNumberTypes;
	    private User user;
	    private List<PhoneNumber> removedPhoneNumbers = new List<PhoneNumber>(); 

        public EditUserProfile ()
		{
			InitializeComponent ();
            PhoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            load();
        }

        private void PhoneNumberStackLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PhoneNumbersLayout.Children.Clear();
            foreach (StackLayout s in PhoneNumberStackLayouts)
            {
                s.Children[2].IsVisible = false;
                s.Children[3].IsVisible = true;
                s.ClassId = PhoneNumberStackLayouts.IndexOf(s).ToString();
                PhoneNumbersLayout.Children.Add(s);
            }
            PhoneNumberStackLayouts.Last().Children[2].IsVisible = true;
            PhoneNumberStackLayouts.Last().Children[3].IsVisible = false;
        }

        private async void load()
        {
            UserRestClient userRestClient = new UserRestClient();
            user = await userRestClient.GetAsyncById(App.Connected.Id);
            Name.Text = user.Name;
            LastName.Text = user.LastName;
            Email.Text = user.Email;
            Password.Text = user.Password;
            Address.ClassId = user.Address_FK.ToString();
            Number.Text = user.Address.Number.ToString();
            Street.Text = user.Address.Street;
            City.Text = user.Address.City;
            ZipCode.Text = user.Address.ZipCode.ToString();
            State.Text = user.Address.State;
            Country.Text = user.Address.Country;
            phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
            foreach (var phoneNumber in user.PhoneNumbers)
            {
                StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(phoneNumber);
                PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
            }
            StackLayout lastPhoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
            PhoneNumberStackLayouts.Add(lastPhoneNumberStackLayout);
        }

        private StackLayout CreatePhoneNumberStackLayout(PhoneNumber phoneNumber)
        {
            Entry phoneNumberEnry = new Entry
            {
                PlaceholderColor = Color.Gray,
                TextColor = Color.Black,
                Placeholder = "xx xxx xxx",
                Text = phoneNumber != null ? phoneNumber.Number : "",
                WidthRequest = 150,
                FontSize = 25,
                Keyboard = Keyboard.Telephone,
                ClassId = phoneNumber!= null ? phoneNumber?.ID.ToString() : "",
            };

            Picker typePicker = new Picker()
            {
                WidthRequest = 90,
                ItemsSource = phoneNumberTypes,
                SelectedIndex = phoneNumber != null? phoneNumberTypes.IndexOf(phoneNumberTypes.FirstOrDefault(t => t.ID == phoneNumber.PhoneNumberType_FK)) : 0,
            };
            Image addIcon = new Image()
            {
                Source = "add_Icon.png",
                WidthRequest = 20,
                IsVisible = phoneNumber == null
            };
            Image removeIcon = new Image()
            {
                Source = "delete.png",
                WidthRequest = 20,
                IsVisible = phoneNumber != null
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
            string removedClassId = (((sender as Image).Parent as StackLayout).Children[0] as Entry).ClassId;
            int removedID = removedClassId == "" ? -1 : Int32.Parse(removedClassId);
            var phoneNumber = user.PhoneNumbers.FirstOrDefault(p => p.ID == removedID);
            if (phoneNumber!= null)
            {
                removedPhoneNumbers.Add(phoneNumber);
            }
        }

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            (PhoneNumberStackLayouts.Last().Children[2] as Image).IsVisible = false;
            (PhoneNumberStackLayouts.Last().Children[3] as Image).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
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
            else if (LastName.Text == null || LastName.Text == "")
            {
                await DisplayAlert("Erreur", "Le champ Prenom est obligateur!", "Ok");
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

        public async void UpdateBt_Clicked(object sender, EventArgs e)
        {
            if (await valid())
            {
                await PopupNavigation.PushAsync(new LoadingPopupPage());
                UserRestClient userRC = new UserRestClient();
                RestClient<Address> addressRC = new RestClient<Address>();
                //RestClient<PhoneNumber> phoneNumberRC = new RestClient<PhoneNumber>();
                if (this.user.Email != Email.Text.ToLower() && await userRC.GetAsyncByEmail(Email.Text.ToLower()) != null)
                {
                    await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                    Email.Text = "";
                    await PopupNavigation.PopAsync();
                    return;
                }

                Address address = new Address()
                {
                    ID = Int32.Parse(Address.ClassId),
                    Number = Int32.Parse(Number.Text),
                    Street = Street.Text,
                    City = City.Text,
                    State = State.Text,
                    Country = Country.Text,
                    ZipCode = Int32.Parse(ZipCode.Text)
                };
                await addressRC.PutAsync(address.ID, address);

                User user = new User()
                {
                    ID = this.user.ID,
                    Name = Name.Text.ToLower(),
                    LastName = LastName.Text.ToLower(),
                    Email = Email.Text.ToLower(),
                    Password = Password.Text,
                    Address_FK = address.ID
                };
                await userRC.PutAsync(user.ID, user);
                RestClient<PhoneNumber> phoneNumberRestClient = new RestClient<PhoneNumber>();
                foreach (var removedPhoneNumber in removedPhoneNumbers)
                {
                    await phoneNumberRestClient.DeleteAsync(removedPhoneNumber.ID);
                }
                foreach (var phoneNumberStackLayout in PhoneNumberStackLayouts)
                {
                    Entry phoneNumberEntry = (phoneNumberStackLayout.Children[0] as Entry);
                    if (phoneNumberEntry.Text != "")
                    {
                        PhoneNumber p = new PhoneNumber()
                        {
                            Number = phoneNumberEntry.Text,
                            PhoneNumberType_FK = (phoneNumberTypes.ElementAt((phoneNumberStackLayout.Children[1] as Picker).SelectedIndex)).ID,
                        };
                        if (phoneNumberEntry.ClassId != "")
                        {
                            int phoneNumberID = Int32.Parse(phoneNumberEntry.ClassId);
                            p.ID = phoneNumberID;
                            await phoneNumberRestClient.PutAsync(p.ID, p);
                        }
                        else
                        {
                            p.User = user;
                            await phoneNumberRestClient.PostAsync(p);
                        }
                    }
                }
            }
            await DisplayAlert("Succées", "Votre profil à été mis à jour!", "Ok");
            await PopupNavigation.PopAsync();
            await Navigation.PopAsync();
        }

	    private async void DeleteBt_Clicked(object sender, EventArgs e)
	    {
            if (user.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
            {
                await
                    DisplayAlert("Erreur",
                        "Impossible de supprimer votre compte, une ou plusieurs commandes ne sont pas encore réglées!",
                        "Ok");
                return;
            }
            await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer votre compte?", "Oui", "Annuler");
	        var userRC = new RestClient<User>();
            if (await userRC.DeleteAsync(user.ID))
            {
                await DisplayAlert("Succées", "Votre Compte a été supprimer.\n", "Ok");
                App.Logout();
            }
        }
	}
}
