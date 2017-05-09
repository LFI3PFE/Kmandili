using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.SignIn;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
	    private MediaFile _mediaFileCover;
	    private MediaFile _mediaFileProfile;


        private ToolbarItem changeProfilePicToolbarItem;
	    private ToolbarItem changeCoverPicToolbarItem;
	    private ToolbarItem categoriesToolbarItem;

        private ToolbarItem cancelChangeProfilePicToolbarItem;
        private ToolbarItem cancelChangeCoverPicToolbarItem;

        private PastryShopMasterDetailPage pastryShopMasterDetailPage;
	    public bool UpdateParent = false;

        public EditProfileInfo (PastryShopMasterDetailPage pastryShopMasterDetailPage)
		{
			InitializeComponent ();
            this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
            changeProfilePicToolbarItem = new ToolbarItem()
            {
                Text = "Changher la photo de profile",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0,
            };
            changeProfilePicToolbarItem.Clicked += ChangeProfilePicToolbarItem_Clicked;

            changeCoverPicToolbarItem = new ToolbarItem()
            {
                Text = "Changher la photo de couverture",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1,
            };
            changeCoverPicToolbarItem.Clicked += ChangeCoverPicToolbarItem_Clicked;

            cancelChangeProfilePicToolbarItem = new ToolbarItem()
            {
                Text = "Annuler le changement de la photo de profile",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0,
            };
            cancelChangeProfilePicToolbarItem.Clicked += CancelChangeProfilePicToolbarItem_Clicked;

            cancelChangeCoverPicToolbarItem = new ToolbarItem()
            {
                Text = "Annuler le changement de la photo de couverture",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1
            };
            cancelChangeCoverPicToolbarItem.Clicked += CancelChangeCoverPicToolbarItem_Clicked;

            categoriesToolbarItem = new ToolbarItem()
            {
                Text = "Categories",
                Order = ToolbarItemOrder.Primary,
                Priority = 1,
                Icon = "categories.png"
            };
            categoriesToolbarItem.Clicked += CategoriesToolbarItem_Clicked;

            ToolbarItems.Add(changeProfilePicToolbarItem);
            ToolbarItems.Add(changeCoverPicToolbarItem);
            ToolbarItems.Add(categoriesToolbarItem);
            PhoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            load();
        }

        private async void CategoriesToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new EditCategories(this, pastryShop));
        }

        private void CancelChangeCoverPicToolbarItem_Clicked(object sender, EventArgs e)
        {
            _mediaFileCover = null;
            ToolbarItems.Remove(cancelChangeCoverPicToolbarItem);
            ToolbarItems.Add(changeCoverPicToolbarItem);
        }

        private void CancelChangeProfilePicToolbarItem_Clicked(object sender, EventArgs e)
        {
            _mediaFileProfile = null;
            ToolbarItems.Remove(cancelChangeProfilePicToolbarItem);
            ToolbarItems.Add(changeProfilePicToolbarItem);
        }

        private async void ChangeCoverPicToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.galleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileCover = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileCover == null)
                return;
            ToolbarItems.Remove(changeCoverPicToolbarItem);
            ToolbarItems.Add(cancelChangeCoverPicToolbarItem);
            App.galleryIsOpent = false;
        }

        private async void ChangeProfilePicToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.galleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfile = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfile == null)
                return;
            ToolbarItems.Remove(changeProfilePicToolbarItem);
            ToolbarItems.Add(cancelChangeProfilePicToolbarItem);
            App.galleryIsOpent = false;
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

        public async void load()
        {
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            pastryShop = await pastryShopRC.GetAsyncById(App.Connected.Id);
            if (pastryShop == null) return;
            phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
            priceRanges = await priceRangeTypeRC.GetAsync();
            if (phoneNumberTypes == null || priceRanges == null) return;
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
            if (string.IsNullOrEmpty((((sender as Image).Parent as StackLayout).Children[0] as Entry).Text)) return;
            (PhoneNumberStackLayouts.Last().Children[2] as Image).IsVisible = false;
            (PhoneNumberStackLayouts.Last().Children[3] as Image).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout(null);
            PhoneNumberStackLayouts.Add(phoneNumberStackLayout);
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

	    public async void EmailVerified()
	    {
            UpdateBt.IsEnabled = false;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRC = new PastryShopRestClient();
            RestClient<Address> addressRC = new RestClient<Address>();
            RestClient<PhoneNumber> phoneNumberRC = new RestClient<PhoneNumber>();

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
            if (!(await addressRC.PutAsync(address.ID, address))) return;

            if (_mediaFileProfile != null)
            {
                string newURL = await Upload(_mediaFileProfile);
                if (!string.IsNullOrEmpty(newURL) && (await Delete(pastryShop.ProfilePic)))
                {
                    pastryShop.ProfilePic = newURL;
                }
            }

            if (_mediaFileCover != null)
            {
                string newURL = await Upload(_mediaFileProfile);
                if (!string.IsNullOrEmpty(newURL) && (await Delete(pastryShop.CoverPic)))
                {
                    pastryShop.CoverPic = newURL;
                }
            }

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
                NumberOfRatings = pastryShop.NumberOfRatings,
                RatingSum = pastryShop.RatingSum,
                ProfilePic = pastryShop.ProfilePic,
                CoverPic = pastryShop.CoverPic,
            };
            if (!(await pastryShopRC.PutAsync(newPastryShop.ID, newPastryShop))) return;
            foreach (var removedPhoneNumber in removedPhoneNumbers)
            {
                if (!(await phoneNumberRC.DeleteAsync(removedPhoneNumber.ID))) return;
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
                        if (!(await phoneNumberRC.PutAsync(p.ID, p))) return;
                    }
                    else
                    {
                        p.PastryShop = newPastryShop;
                        if (await phoneNumberRC.PostAsync(p) == null) return;
                    }
                }
            }
            await DisplayAlert("Succées", "Votre profil à été mis à jour!", "Ok");
            await PopupNavigation.PopAsync();
            UpdateParent = true;
            await Navigation.PopAsync();
        }

        private async void UpdateBt_Clicked(object sender, EventArgs e)
        {
            if (await valid())
            {
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                UserRestClient userRC = new UserRestClient();
                if (pastryShop.Email != Email.Text.ToLower())
                {
                    if ((await pastryShopRC.GetAsyncByEmail(Email.Text.ToLower()) != null) || (await  userRC.GetAsyncByEmail(Email.Text.ToLower()) != null))
                    {
                        await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                        Email.Text = "";
                        return;
                    }
                    await PopupNavigation.PushAsync(new EmailVerificationPopupPage(this, Email.Text.ToLower()));
                }
                else
                {
                    EmailVerified();
                }
            }
        }

	    protected override void OnDisappearing()
	    {
	        if (UpdateParent)
	        {
                pastryShopMasterDetailPage.ReloadPastryShop();
            }
	    }

	    private async Task<string> Upload(MediaFile upfile)
        {
            string fileName = Guid.NewGuid().ToString();
            var stream = upfile.GetStream();
            var res = await new UploadRestClient().Upload(stream, fileName);
            if (res)
            {
                return App.ServerURL + "Uploads/" + fileName + ".jpg";
            }
            return null;
        }

	    private async Task<bool> Delete(string picURL)
	    {
            string fileName = picURL.Substring(44, (picURL.Length - 44));
	        fileName = fileName.Substring(0, (fileName.Length - 4));
            UploadRestClient uploadRC = new UploadRestClient();
            return (await uploadRC.Delete(fileName));
	    }

	    private async void DeleteBt_Clicked(object sender, EventArgs e)
	    {
	        if (pastryShop.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
	        {
	            await
	                DisplayAlert("Erreur",
	                    "Impossible de supprimer votre compte, une ou plusieurs commandes ne sont pas encore réglées!",
	                    "Ok");
                return;
	        }
            await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer votre compte?", "Oui", "Annuler");
            var pastryShopRC = new PastryShopRestClient();
	        if (await pastryShopRC.DeleteAsync(pastryShop.ID))
	        {
	            await DisplayAlert("Succées", "Votre Compte a été supprimer.\n", "Ok");
                App.Logout();
	        }
	    }
	}
}
