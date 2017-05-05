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

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditProfileInfo : ContentPage
	{
        private ObservableCollection<StackLayout> PhoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private RestClient<PhoneNumberType> phoneNumberTypeRC = new RestClient<PhoneNumberType>();
        private RestClient<PriceRange> priceRangeTypeRC = new RestClient<PriceRange>();
        private List<PhoneNumberType> phoneNumberTypes;
        private List<PriceRange> priceRanges;
	    private PastryShop pastryShop;
        private List<PhoneNumber> removedPhoneNumbers = new List<PhoneNumber>();

        public EditProfileInfo ()
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
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(App.Connected.Id);
            phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
            priceRanges = await priceRangeTypeRC.GetAsync();
            PriceRange.ItemsSource = priceRanges;

            Name.Text = pastryShop.Name;
            Email.Text = pastryShop.Email;
            Password.Text = pastryShop.Password;
            ShortDesc.Text = pastryShop.ShortDesc;
            LongDesc.Text = pastryShop.LongDesc;
            LongDesc.TextColor = Color.Black;
            PriceRange.SelectedIndex = priceRanges.IndexOf(priceRanges.FirstOrDefault(pr => pr.ID == pastryShop.PriceRange_FK));
            Address.ClassId = pastryShop.Address_FK.ToString();
            Number.Text = pastryShop.Address.Number.ToString();
            Street.Text = pastryShop.Address.Street;
            City.Text = pastryShop.Address.City;
            ZipCode.Text = pastryShop.Address.ZipCode.ToString();
            State.Text = pastryShop.Address.State;
            Country.Text = pastryShop.Address.Country;

            foreach (var phoneNumber in pastryShop.PhoneNumbers)
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
                ClassId = phoneNumber != null ? phoneNumber?.ID.ToString() : "",
            };

            Picker typePicker = new Picker()
            {
                WidthRequest = 90,
                ItemsSource = phoneNumberTypes,
                SelectedIndex = phoneNumber != null ? phoneNumberTypes.IndexOf(phoneNumberTypes.FirstOrDefault(t => t.ID == phoneNumber.PhoneNumberType_FK)) : 0,
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
            var phoneNumber = pastryShop.PhoneNumbers.FirstOrDefault(p => p.ID == removedID);
            if (phoneNumber != null)
            {
                removedPhoneNumbers.Add(phoneNumber);
            }
        }

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            if ((((sender as Image).Parent as StackLayout).Children[0] as Entry).Text != "")
            {
                (PhoneNumberStackLayouts.Last().Children[2] as Image).IsVisible = false;
                (PhoneNumberStackLayouts.Last().Children[3] as Image).IsVisible = true;
                StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
                PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
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

        private void editorFocused(object sender, EventArgs e)
        {
            if (LongDesc.Text == "Longue Description...")
            {
                LongDesc.Text = "";
                LongDesc.TextColor = Color.Black;
            }
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

        private async void UpdateBt_Clicked(object sender, EventArgs e)
        {
            UpdateBt.IsEnabled = false;
            if (await valid())
            {
                await PopupNavigation.PushAsync(new LoadingPopupPage());
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                RestClient<Address> addressRC = new RestClient<Address>();
                RestClient<PhoneNumber> phoneNumberRC = new RestClient<PhoneNumber>();

                if (pastryShop.Email != Email.Text.ToLower() && await pastryShopRC.GetAsyncByEmail(Email.Text.ToLower()) != null)
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

                PastryShop newPastryShop = new PastryShop()
                {
                    ID = pastryShop.ID,
                    Name = Name.Text,
                    Email = Email.Text.ToLower(),
                    Password = Password.Text,
                    Address_FK = address.ID,
                    LongDesc = LongDesc.Text,
                    ShortDesc = ShortDesc.Text,
                    PriceRange_FK = priceRanges.ElementAt(PriceRange.SelectedIndex).ID,
                    NumberOfRatings =  pastryShop.NumberOfRatings,
                    RatingSum = pastryShop.RatingSum,
                    ProfilePic = pastryShop.ProfilePic,
                    CoverPic = pastryShop.CoverPic,
                };
                await pastryShopRC.PutAsync(newPastryShop.ID, newPastryShop);
                foreach (var removedPhoneNumber in removedPhoneNumbers)
                {
                    await phoneNumberRC.DeleteAsync(removedPhoneNumber.ID);
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
                            await phoneNumberRC.PutAsync(p.ID, p);
                        }
                        else
                        {
                            p.PastryShop = newPastryShop;
                            await phoneNumberRC.PostAsync(p);
                        }
                    }
                    //foreach (StackLayout s in PhoneNumberStackLayouts)
                    //{
                    //    if ((s.Children[0] as Entry).Text != "")
                    //    {
                    //        PhoneNumber p = new PhoneNumber()
                    //        {
                    //            Number = (s.Children[0] as Entry).Text,
                    //            PhoneNumberType_FK = (phoneNumberTypes.ElementAt((s.Children[1] as Picker).SelectedIndex)).ID,
                    //        };
                    //        pastryShop.PhoneNumbers.Add(p);
                    //    }
                    //}
                }
                await DisplayAlert("Succées", "Votre profil à été mis à jour!", "Ok");
                await PopupNavigation.PopAsync();
                await Navigation.PopAsync();
            }

        }

    }
}
