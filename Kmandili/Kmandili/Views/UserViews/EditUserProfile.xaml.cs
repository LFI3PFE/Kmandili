﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditUserProfile
	{
        private readonly ObservableCollection<StackLayout> _phoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private readonly RestClient<PhoneNumberType> _phoneNumberTypeRc = new RestClient<PhoneNumberType>();
        private List<PhoneNumberType> _phoneNumberTypes;
	    private User _user;
	    private readonly List<PhoneNumber> _removedPhoneNumbers = new List<PhoneNumber>(); 

        public EditUserProfile (int id)
		{
			InitializeComponent ();
            _phoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            Load(id);
        }

        private void PhoneNumberStackLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PhoneNumbersLayout.Children.Clear();
            foreach (StackLayout s in _phoneNumberStackLayouts)
            {
                s.Children[2].IsVisible = false;
                s.Children[3].IsVisible = true;
                s.ClassId = _phoneNumberStackLayouts.IndexOf(s).ToString();
                PhoneNumbersLayout.Children.Add(s);
            }
            _phoneNumberStackLayouts.Last().Children[2].IsVisible = true;
            _phoneNumberStackLayouts.Last().Children[3].IsVisible = false;
        }

        private async void Load(int id)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            UserRestClient userRestClient = new UserRestClient();
            try
            {
                _user = await userRestClient.GetAsyncById(id);
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
            if (_user == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            Name.Text = _user.Name;
            LastName.Text = _user.LastName;
            Email.Text = _user.Email;
            Password.Text = _user.Password;
            Address.ClassId = _user.Address_FK.ToString();
            Number.Text = _user.Address.Number.ToString();
            Street.Text = _user.Address.Street;
            City.Text = _user.Address.City;
            ZipCode.Text = _user.Address.ZipCode.ToString();
            State.Text = _user.Address.State;
            Country.Text = _user.Address.Country;
            try
            {
                _phoneNumberTypes = await _phoneNumberTypeRc.GetAsync();
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
            if (_phoneNumberTypes == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            foreach (var phoneNumber in _user.PhoneNumbers)
            {
                StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(phoneNumber);
                _phoneNumberStackLayouts.Add(phoneNumberStackLayout);
            }
            StackLayout lastPhoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
            _phoneNumberStackLayouts.Add(lastPhoneNumberStackLayout);
            await PopupNavigation.PopAsync();
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
                ClassId = phoneNumber?.ID.ToString() ?? ""
            };

            Picker typePicker = new Picker()
            {
                WidthRequest = 90,
                ItemsSource = _phoneNumberTypes,
                SelectedIndex = phoneNumber != null? _phoneNumberTypes.IndexOf(_phoneNumberTypes.FirstOrDefault(t => t.ID == phoneNumber.PhoneNumberType_FK)) : 0,
            };
            Image addIcon = new Image()
            {
                Source = "add.png",
                WidthRequest = 20,
                IsVisible = phoneNumber == null
            };
            Image removeIcon = new Image()
            {
                Source = "delete.png",
                WidthRequest = 20,
                IsVisible = phoneNumber != null
            };
            TapGestureRecognizer addPhoneNumberGr = new TapGestureRecognizer();
            addPhoneNumberGr.Tapped += AddPhoneNumber_Tapped;
            addIcon.GestureRecognizers.Add(addPhoneNumberGr);

            TapGestureRecognizer removePhoneNumberGr = new TapGestureRecognizer();
            removePhoneNumberGr.Tapped += RemovePhoneNumberGR_Tapped;
            removeIcon.GestureRecognizers.Add(removePhoneNumberGr);

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
            int index = Int32.Parse((((Image) sender).Parent as StackLayout)?.ClassId ?? throw new InvalidOperationException());
            _phoneNumberStackLayouts.RemoveAt(index);
            string removedClassId = ((((Image) sender).Parent as StackLayout)?.Children[0] as Entry)?.ClassId;
            int removedId = removedClassId == "" ? -1 : Int32.Parse(removedClassId ?? throw new InvalidOperationException());
            var phoneNumber = _user.PhoneNumbers.FirstOrDefault(p => p.ID == removedId);
            if (phoneNumber!= null)
            {
                _removedPhoneNumbers.Add(phoneNumber);
            }
        }

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((((sender as Image)?.Parent as StackLayout)?.Children[0] as Entry)?.Text)) return;
            ((Image) _phoneNumberStackLayouts.Last().Children[2]).IsVisible = false;
            ((Image) _phoneNumberStackLayouts.Last().Children[3]).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
            _phoneNumberStackLayouts.Add(phoneNumberStackLayout);
        }

        private async Task<bool> ValidAddress()
        {
            if (string.IsNullOrEmpty(Number.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Numero est obligateur!", "Ok");
                return false;
            }
            else if (!int.TryParse(Number.Text, out _))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment ne doit contenir que des Chiffres!", "Ok");
                Number.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(Street.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Rue est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(City.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Ville est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(ZipCode.Text))
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal est obligateur!", "Ok");
                return false;
            }
            else if (ZipCode.Text.Length != 4)
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Co. Postal doit contenir exactement 4 Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (!int.TryParse(ZipCode.Text, out _))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Co. Postal ne doit contenir que des Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(State.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Gouvernorat est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(Country.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Pays est obligateur!", "Ok");
                return false;
            }
            return true;
        }

        private async Task<bool> ValidPhoneNumber()
        {
            bool exist = false;
            foreach (StackLayout s in _phoneNumberStackLayouts)
            {
                string phoneNumber = (s.Children[0] as Entry)?.Text;
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    if (!int.TryParse(phoneNumber, out _))
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (_phoneNumberStackLayouts.IndexOf(s) + 1) + " ne doit contenir que des chiffres!", "Ok");
                        ((Entry) s.Children[0]).Text = "";
                        return false;
                    }
                    else if (phoneNumber.Length != 8)
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (_phoneNumberStackLayouts.IndexOf(s) + 1) + " doit contenir exactement 8 chiffres!", "Ok");
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
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Au moins un Numéro de Téléphone est obligatoir!", "Ok");
            }
            return exist;
        }

        private async Task<bool> Valid()
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Nom est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(LastName.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Prenom est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(Email.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Email est obligateur!", "Ok");
                return false;
            }
            else if (!App.IsValidEmail(Email.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Email est invalide!", "Ok");
                Email.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(Password.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Password est obligateur!", "Ok");
                return false;
            }
            else if (!(await ValidAddress()))
            {
                return false;
            }
            else if (!(await ValidPhoneNumber()))
            {
                return false;
            }
            return true;
        }


        public async void EmailVerified()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            UserRestClient userRc = new UserRestClient();
            RestClient<Address> addressRc = new RestClient<Address>();
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
            try
            {
                if (!(await addressRc.PutAsync(address.ID, address))) return;
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

            User user = new User()
            {
                ID = _user.ID,
                Name = Name.Text.ToLower(),
                LastName = LastName.Text.ToLower(),
                Email = Email.Text.ToLower(),
                Password = Password.Text,
                Address_FK = address.ID
            };
            try
            {
                if (!(await userRc.PutAsync(user.ID, user))) return;
                Settings.Email = Email.Text.ToLower();
                Settings.Password = Password.Text;
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
            RestClient<PhoneNumber> phoneNumberRestClient = new RestClient<PhoneNumber>();
            try
            {
                foreach (var removedPhoneNumber in _removedPhoneNumbers)
                {
                    if (!(await phoneNumberRestClient.DeleteAsync(removedPhoneNumber.ID))) return;
                }
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
            
            foreach (var phoneNumberStackLayout in _phoneNumberStackLayouts)
            {
                Entry phoneNumberEntry = (phoneNumberStackLayout.Children[0] as Entry);
                if (phoneNumberEntry?.Text != "")
                {
                    PhoneNumber p = new PhoneNumber()
                    {
                        Number = phoneNumberEntry?.Text,
                        PhoneNumberType_FK = (_phoneNumberTypes.ElementAt(((Picker) phoneNumberStackLayout.Children[1]).SelectedIndex)).ID,
                    };
                    if (phoneNumberEntry?.ClassId != "")
                    {
                        int phoneNumberId = Int32.Parse(phoneNumberEntry?.ClassId ?? throw new InvalidOperationException());
                        p.ID = phoneNumberId;
                        try
                        {
                            if (!(await phoneNumberRestClient.PutAsync(p.ID, p))) return;
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
                    }
                    else
                    {
                        p.User = user;
                        try
                        {
                            if (await phoneNumberRestClient.PostAsync(p) == null) return;
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
                    }
                }
            }
            await DisplayAlert("Succées", "Votre profil à été mis à jour!", "Ok");
            await PopupNavigation.PopAllAsync();
            await Navigation.PopAsync();
        }

        public async void UpdateBt_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (await Valid())
            {
                if (_user.Email != Email.Text.ToLower())
                {
                    try
                    {
                        if ((await App.GetEmailExist(Email.Text.ToLower())))
                        {
                            await PopupNavigation.PopAllAsync();
                            await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                            Email.Text = "";
                            return;
                        }
                        await PopupNavigation.PopAllAsync();
                        await PopupNavigation.PushAsync(new UEmailVerificationPopupPage(this, Email.Text.ToLower()));
                    }
                    catch (Exception)
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                    }
                }
                else
                {
                    await PopupNavigation.PopAllAsync();
                    EmailVerified();
                }
            }
        }

	    private async void DeleteBt_Clicked(object sender, EventArgs e)
	    {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (_user.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Impossible de supprimer votre compte, une ou plusieurs commandes ne sont pas encore réglées!",
                        "Ok");
                return;
            }
            if(!await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer votre compte?", "Oui", "Annuler"))
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
	        var userRc = new RestClient<User>();
	        try
	        {
                if (await userRc.DeleteAsync(_user.ID))
                {
                    await PopupNavigation.PopAllAsync();
                    await DisplayAlert("Succées", "Votre Compte a été supprimer.\n", "Ok");
                    App.Logout();
                }
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                await Navigation.PopAsync();
            }
        }
	}
}
