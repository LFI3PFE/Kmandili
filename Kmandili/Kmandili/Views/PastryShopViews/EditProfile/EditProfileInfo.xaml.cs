using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmandili.Helpers;
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
	public partial class EditProfileInfo
	{
        private readonly ObservableCollection<StackLayout> _phoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private readonly RestClient<PhoneNumberType> _phoneNumberTypeRc = new RestClient<PhoneNumberType>();
        private readonly RestClient<PriceRange> _priceRangeTypeRc = new RestClient<PriceRange>();
        private List<PhoneNumberType> _phoneNumberTypes;
        private List<PriceRange> _priceRanges;
	    private PastryShop _pastryShop;
        private readonly List<PhoneNumber> _removedPhoneNumbers = new List<PhoneNumber>();
	    private MediaFile _mediaFileCover;
	    private MediaFile _mediaFileProfile;


        private readonly ToolbarItem _changeProfilePicToolbarItem;
	    private readonly ToolbarItem _changeCoverPicToolbarItem;

	    private readonly ToolbarItem _cancelChangeProfilePicToolbarItem;
        private readonly ToolbarItem _cancelChangeCoverPicToolbarItem;

	    private readonly int _id;

        public EditProfileInfo (int id, bool showDelete)
		{
		    InitializeComponent ();
            _id = id;
            DeleteBt.IsVisible = showDelete;

            var methodeToolbarItem = new ToolbarItem()
            {
                Icon = "delevery.png",
                Text = "Methodes de livraisons",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };

            methodeToolbarItem.Clicked += MethodeToolbarItem_Clicked;

            _changeProfilePicToolbarItem = new ToolbarItem()
            {
                Text = "Changer la photo de profile",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0,
            };
            _changeProfilePicToolbarItem.Clicked += ChangeProfilePicToolbarItem_Clicked;

            _changeCoverPicToolbarItem = new ToolbarItem()
            {
                Text = "Changer la photo de couverture",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1,
            };
            _changeCoverPicToolbarItem.Clicked += ChangeCoverPicToolbarItem_Clicked;

            _cancelChangeProfilePicToolbarItem = new ToolbarItem()
            {
                Text = "Annuler le changement de la photo de profile",
                Order = ToolbarItemOrder.Secondary,
                Priority = 0,
            };
            _cancelChangeProfilePicToolbarItem.Clicked += CancelChangeProfilePicToolbarItem_Clicked;

            _cancelChangeCoverPicToolbarItem = new ToolbarItem()
            {
                Text = "Annuler le changement de la photo de couverture",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1
            };
            _cancelChangeCoverPicToolbarItem.Clicked += CancelChangeCoverPicToolbarItem_Clicked;

            var categoriesToolbarItem = new ToolbarItem()
            {
                Text = "Categories",
                Order = ToolbarItemOrder.Primary,
                Priority = 1,
                Icon = "categories.png"
            };
            categoriesToolbarItem.Clicked += CategoriesToolbarItem_Clicked;

            ToolbarItems.Add(_changeProfilePicToolbarItem);
            ToolbarItems.Add(_changeCoverPicToolbarItem);
            ToolbarItems.Add(categoriesToolbarItem);
            ToolbarItems.Add(methodeToolbarItem);
            _phoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            Load();
        }

        private async void MethodeToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditDeleveryMethods(this, _pastryShop.ID));
        }

        private async void CategoriesToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new EditCategories(this, _pastryShop));
        }

        private void CancelChangeCoverPicToolbarItem_Clicked(object sender, EventArgs e)
        {
            _mediaFileCover = null;
            ToolbarItems.Remove(_cancelChangeCoverPicToolbarItem);
            ToolbarItems.Add(_changeCoverPicToolbarItem);
        }

        private void CancelChangeProfilePicToolbarItem_Clicked(object sender, EventArgs e)
        {
            _mediaFileProfile = null;
            ToolbarItems.Remove(_cancelChangeProfilePicToolbarItem);
            ToolbarItems.Add(_changeProfilePicToolbarItem);
        }

        private async void ChangeCoverPicToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.GalleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileCover = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileCover == null)
                return;
            ToolbarItems.Remove(_changeCoverPicToolbarItem);
            ToolbarItems.Add(_cancelChangeCoverPicToolbarItem);
            App.GalleryIsOpent = false;
        }

        private async void ChangeProfilePicToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.GalleryIsOpent = true;
            await CrossMedia.Current.Initialize();

            _mediaFileProfile = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFileProfile == null)
                return;
            ToolbarItems.Remove(_changeProfilePicToolbarItem);
            ToolbarItems.Add(_cancelChangeProfilePicToolbarItem);
            App.GalleryIsOpent = false;
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
            if(!_phoneNumberStackLayouts.Any()) return;
            _phoneNumberStackLayouts.Last().Children[2].IsVisible = true;
            _phoneNumberStackLayouts.Last().Children[3].IsVisible = false;
        }

        public async void Load()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRc = new PastryShopRestClient();
            try
            {
                _pastryShop = await pastryShopRc.GetAsyncById(_id);
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
            if (_pastryShop == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            try
            {
                _phoneNumberTypes = await _phoneNumberTypeRc.GetAsync();
                _priceRanges = await _priceRangeTypeRc.GetAsync();
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
            if (_phoneNumberTypes == null || _priceRanges == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            PriceRange.ItemsSource = _priceRanges;

            Name.Text = _pastryShop.Name;
            Email.Text = _pastryShop.Email;
            Password.Text = _pastryShop.Password;
            ShortDesc.Text = _pastryShop.ShortDesc;
            LongDesc.Text = _pastryShop.LongDesc;
            LongDesc.TextColor = Color.Black;
            PriceRange.SelectedIndex = _priceRanges.IndexOf(_priceRanges.FirstOrDefault(pr => pr.ID == _pastryShop.PriceRange_FK));
            Address.ClassId = _pastryShop.Address_FK.ToString();
            Number.Text = _pastryShop.Address.Number.ToString();
            Street.Text = _pastryShop.Address.Street;
            City.Text = _pastryShop.Address.City;
            ZipCode.Text = _pastryShop.Address.ZipCode.ToString();
            State.Text = _pastryShop.Address.State;
            Country.Text = _pastryShop.Address.Country;

            _phoneNumberStackLayouts.Clear();
            foreach (var phoneNumber in _pastryShop.PhoneNumbers)
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
                ClassId = phoneNumber != null ? phoneNumber.ID.ToString() : ""
            };

            Picker typePicker = new Picker()
            {
                WidthRequest = 90,
                ItemsSource = _phoneNumberTypes,
                SelectedIndex = phoneNumber != null ? _phoneNumberTypes.IndexOf(_phoneNumberTypes.FirstOrDefault(t => t.ID == phoneNumber.PhoneNumberType_FK)) : 0,
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
            int index = Int32.Parse(((sender as Image)?.Parent as StackLayout)?.ClassId);
            _phoneNumberStackLayouts.RemoveAt(index);
            string removedClassId = (((sender as Image)?.Parent as StackLayout)?.Children[0] as Entry)?.ClassId;
            int removedId = removedClassId == "" ? -1 : Int32.Parse(removedClassId);
            var phoneNumber = _pastryShop.PhoneNumbers.FirstOrDefault(p => p.ID == removedId);
            if (phoneNumber != null)
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

        private void ShortDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ShortDesc.Text;
            if (text.Length > 30)
            {
                text = text.Remove(text.Length - 1);
                ShortDesc.Text = text;
            }
        }

        private void LongDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = LongDesc.Text;
            if (text.Length > 100)
            {
                text = text.Remove(text.Length - 1);
                LongDesc.Text = text;
            }
        }

        private void EditorFocused(object sender, EventArgs e)
        {
            if (LongDesc.Text == "Longue Description...")
            {
                LongDesc.Text = "";
                LongDesc.TextColor = Color.Black;
            }
        }

        private void EditorUnFocused(object sender, EventArgs e)
        {
            if (LongDesc.Text == "")
            {
                LongDesc.Text = "Longue Description...";
                LongDesc.TextColor = Color.Gray;
            }
        }

        private async Task<bool> ValidAddress()
        {
            if (string.IsNullOrEmpty(Number.Text))
            {
                await DisplayAlert("Erreur", "Le champ Numero est obligateur!", "Ok");
                return false;
            }
            else if (!int.TryParse(Number.Text, out _))
            {
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment ne doit contenir que des Chiffres!", "Ok");
                Number.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(Street.Text))
            {
                await DisplayAlert("Erreur", "Le champ Rue est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(City.Text))
            {
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
                await DisplayAlert("Erreur", "Le champ Co. Postal doit contenir exactement 4 Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (!int.TryParse(ZipCode.Text, out _))
            {
                await DisplayAlert("Erreur", "Le champ Co. Postal ne doit contenir que des Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(State.Text))
            {
                await DisplayAlert("Erreur", "Le champ Gouvernorat est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(Country.Text))
            {
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
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (_phoneNumberStackLayouts.IndexOf(s) + 1) + " ne doit contenir que des chiffres!", "Ok");
                        ((Entry) s.Children[0]).Text = "";
                        return false;
                    }
                    else if (phoneNumber.Length != 8)
                    {
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
                await DisplayAlert("Erreur", "Au moins un Numéro de Téléphone est obligatoir!", "Ok");
            }
            return exist;
        }

        private async Task<bool> Valid()
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                await DisplayAlert("Erreur", "Le champ Nom est obligateur!", "Ok");
                return false;
            }
            else if (string.IsNullOrEmpty(Email.Text))
            {
                await DisplayAlert("Erreur", "Le champ Email est obligateur!", "Ok");
                return false;
            }
            else if (!App.IsValidEmail(Email.Text))
            {
                await DisplayAlert("Erreur", "Le champ Email est invalide!", "Ok");
                Email.Text = "";
                return false;
            }
            else if (string.IsNullOrEmpty(Password.Text))
            {
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
            UpdateBt.IsEnabled = false;
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            PastryShopRestClient pastryShopRc = new PastryShopRestClient();
            RestClient<Address> addressRc = new RestClient<Address>();
            RestClient<PhoneNumber> phoneNumberRc = new RestClient<PhoneNumber>();

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

            if (_mediaFileProfile != null)
            {
                string newUrl = await Upload(_mediaFileProfile);
                if (!string.IsNullOrEmpty(newUrl) && (await Delete(_pastryShop.ProfilePic)))
                {
                    _pastryShop.ProfilePic = newUrl;
                }
            }

            if (_mediaFileCover != null)
            {
                string newUrl = await Upload(_mediaFileProfile);
                if (!string.IsNullOrEmpty(newUrl) && (await Delete(_pastryShop.CoverPic)))
                {
                    _pastryShop.CoverPic = newUrl;
                }
            }

            PastryShop newPastryShop = new PastryShop()
            {
                ID = _pastryShop.ID,
                Name = Name.Text,
                Email = Email.Text.ToLower(),
                Password = Password.Text,
                Address_FK = address.ID,
                LongDesc = LongDesc.Text,
                ShortDesc = ShortDesc.Text,
                PriceRange_FK = _priceRanges.ElementAt(PriceRange.SelectedIndex).ID,
                ProfilePic = _pastryShop.ProfilePic,
                CoverPic = _pastryShop.CoverPic,
            };
	        try
            {
                if (!(await pastryShopRc.PutAsync(newPastryShop.ID, newPastryShop))) return;
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
	        try
            {
                foreach (var removedPhoneNumber in _removedPhoneNumbers)
                {
                    if (!(await phoneNumberRc.DeleteAsync(removedPhoneNumber.ID))) return;
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
                Entry phoneNumberEntry = ((Entry) phoneNumberStackLayout.Children[0]);
                if (phoneNumberEntry != null && phoneNumberEntry.Text != "")
                {
                    PhoneNumber p = new PhoneNumber()
                    {
                        Number = phoneNumberEntry.Text,
                        PhoneNumberType_FK = (_phoneNumberTypes.ElementAt(((Picker) phoneNumberStackLayout.Children[1]).SelectedIndex)).ID,
                    };
                    if (phoneNumberEntry.ClassId != "")
                    {
                        int phoneNumberId = Int32.Parse(phoneNumberEntry.ClassId);
                        p.ID = phoneNumberId;
                        try
                        {
                            if (!(await phoneNumberRc.PutAsync(p.ID, p))) return;
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
                        p.PastryShop = newPastryShop;
                        try
                        {
                            if (await phoneNumberRc.PostAsync(p) == null) return;
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
            await DisplayAlert("Succées", "Compte mis à jour!", "Ok");
            await PopupNavigation.PopAsync();
            await Navigation.PopAsync();
        }

        private async void UpdateBt_Clicked(object sender, EventArgs e)
        {
            if (await Valid())
            {
                if (_pastryShop.Email != Email.Text.ToLower())
                {
                    try
                    {
                        if ((await App.GetEmailExist(Email.Text.ToLower())))
                        {
                            await DisplayAlert("Erreur", "Cette adresse email est déjà utilisée!", "Ok");
                            Email.Text = "";
                            return;
                        }
                        var x = new PEmailVerificationPopupPage(this, Email.Text.ToLower());
                        await PopupNavigation.PushAsync(x);
                    }
                    catch (HttpRequestException)
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                    }
                }
                else
                {
                    EmailVerified();
                }
            }
        }

	    private async Task<string> Upload(MediaFile upfile)
        {
            string fileName = Guid.NewGuid().ToString();
            var stream = upfile.GetStream();
	        try
	        {
                var res = await new UploadRestClient().Upload(stream, fileName);
                if (res)
                {
                    return App.ServerUrl + "Uploads/" + fileName + ".jpg";
                }
                return null;
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
	            return null;
	        }
        }

	    private async Task<bool> Delete(string picUrl)
	    {
            string fileName = picUrl.Substring(App.ServerUrl.Length + 8, (picUrl.Length - (App.ServerUrl.Length + 8)));
	        fileName = fileName.Substring(0, (fileName.Length - 4));
            UploadRestClient uploadRc = new UploadRestClient();
	        try
	        {
                return (await uploadRc.Delete(fileName));
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return false;
	        }
	    }

	    private async void DeleteBt_Clicked(object sender, EventArgs e)
	    {
	        if (_pastryShop.Orders.Any(o => (o.Status_FK != 5 && o.Status_FK != 3)))
	        {
	            await
	                DisplayAlert("Erreur",
	                    "Impossible de supprimer votre compte, une ou plusieurs commandes ne sont pas encore réglées!",
	                    "Ok");
                return;
	        }
            var choix = await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer votre compte?", "Oui", "Annuler");
	        if (!choix) return;
	        var pastryShopRc = new PastryShopRestClient();
	        try
	        {
                if (await pastryShopRc.DeleteAsync(_pastryShop.ID))
                {
                    await DisplayAlert("Succées", "Votre Compte a été supprimer.", "Ok");
                    App.Logout();
                    return;
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
            await DisplayAlert("Erreur", "Une Erreur s'est produite lors de la suppression de votre compte, veuillez réessayer plus tard!.", "Ok");
        }
	}
}
