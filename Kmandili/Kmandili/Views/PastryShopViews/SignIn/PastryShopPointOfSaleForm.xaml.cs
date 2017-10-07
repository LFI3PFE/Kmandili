using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopPointOfSaleForm
	{
        private readonly PastryShop _pastryShop;
        private List<Parking> _parkings = new List<Parking>();
        private readonly ObservableCollection<StackLayout> _phoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private List<PhoneNumberType> _phoneNumberTypes = new List<PhoneNumberType>();
	    public PastryShopEnteringPointOfSales ShopEnteringPointOfSales { get; }

	    public PastryShopPointOfSaleForm(PastryShopEnteringPointOfSales pastryShopEnteringPointOfSales, PastryShop pastryShop)
        {
            InitializeComponent();
            _pastryShop = pastryShop;
            ShopEnteringPointOfSales = pastryShopEnteringPointOfSales;
            _phoneNumberStackLayouts.CollectionChanged += PhoneNumberStackLayouts_CollectionChanged;
            Load();
        }

        private void PhoneNumberStackLayouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PhoneNumbersLayout.Children.Clear();
            foreach (StackLayout s in _phoneNumberStackLayouts)
            {
                s.ClassId = _phoneNumberStackLayouts.IndexOf(s).ToString();
                PhoneNumbersLayout.Children.Add(s);
            }
        }

        private async void Load()
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            CreationDate.MaximumDate = DateTime.Now.Date;
            RestClient<Parking> parkingRc = new RestClient<Parking>();
            try
            {
                _parkings = await parkingRc.GetAsync();
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
            if (_parkings == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            ParkingPicker.ItemsSource = _parkings;
            ParkingPicker.SelectedIndex = 0;

            RestClient<PhoneNumberType> phoneNumberTypeRc = new RestClient<PhoneNumberType>();
            try
            {
                _phoneNumberTypes = await phoneNumberTypeRc.GetAsync();
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
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            _phoneNumberStackLayouts.Add(phoneNumberStackLayout);
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
                ItemsSource = _phoneNumberTypes,
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

        private void AddPhoneNumber_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((((sender as Image)?.Parent as StackLayout)?.Children[0] as Entry)?.Text)) return;
            ((Image) _phoneNumberStackLayouts.Last().Children[2]).IsVisible = false;
            ((Image) _phoneNumberStackLayouts.Last().Children[3]).IsVisible = true;
            StackLayout phoneNumberStackLayout = CreatePhoneNumberStackLayout();
            _phoneNumberStackLayouts.Add(phoneNumberStackLayout);
        }

        private void RemovePhoneNumberGR_Tapped(object sender, EventArgs e)
        {
            int index = Int32.Parse(((sender as Image)?.Parent as StackLayout)?.ClassId);
            _phoneNumberStackLayouts.RemoveAt(index);
        }

        private void RemoveDay(object sender, EventArgs e)
        {
            (((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId = "0";
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[0] as StackLayout).Children[1] as TimePicker).IsEnabled = false;
            ((((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[1] as StackLayout).Children[1] as StackLayout).Children[0] as TimePicker).IsEnabled = false;
            ((((sender as Image).Parent as StackLayout).Parent as StackLayout).Children[1] as Label).TextColor = Color.Gray;
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Gray;
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[1] as StackLayout).Children[0] as Label).TextColor = Color.Gray;
            (sender as Image).IsVisible = false;
            (((sender as Image).Parent as StackLayout).Children[1] as Image).IsVisible = true;
        }

        private void AddDay(object sender, EventArgs e)
        {
            (((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId = "1";
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[0] as StackLayout).Children[1] as TimePicker).IsEnabled = true;
            ((((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[1] as StackLayout).Children[1] as StackLayout).Children[0] as TimePicker).IsEnabled = true;
            ((((sender as Image).Parent as StackLayout).Parent as StackLayout).Children[1] as Label).TextColor = Color.Black;
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Black;
            (((((((sender as Image).Parent as StackLayout).Parent as StackLayout).Parent as Grid).Children[1] as StackLayout).Children[1] as StackLayout).Children[0] as Label).TextColor = Color.Black;
            (sender as Image).IsVisible = false;
            (((sender as Image).Parent as StackLayout).Children[0] as Image).IsVisible = true;
        }

        private async Task<bool> ValidAddress()
        {
            if (string.IsNullOrEmpty(Number.Text))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment est obligateur!", "Ok");
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
                await PopupNavigation.PopAllAsync();
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
                await DisplayAlert("Erreur", "Au moins un Numéro de Téléphone est obligatoir!", "Ok");
                await PopupNavigation.PopAllAsync();
            }
            return exist;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (await ValidAddress() && await ValidPhoneNumber())
            {
                RestClient<PointOfSale> pointOfSaleRc = new RestClient<PointOfSale>();
                RestClient<Address> addressRc = new RestClient<Address>();
                Address address = new Address()
                {
                    Number = Int32.Parse(Number.Text),
                    Street = Street.Text,
                    City = City.Text,
                    State = State.Text,
                    Country = Country.Text,
                    ZipCode = Int32.Parse(ZipCode.Text)
                };
                try
                {
                    address = await addressRc.PostAsync(address);
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
                if (address != null)
                {
                    PointOfSale pointOfSale = new PointOfSale()
                    {
                        CreationDate = CreationDate.Date,
                        ParkingType_FK = _parkings.ElementAt(ParkingPicker.SelectedIndex).ID,
                        PastryShop_FK = _pastryShop.ID,
                        Address_FK = address.ID,
                    };
                    foreach (StackLayout s in _phoneNumberStackLayouts)
                    {
                        if ((s.Children[0] as Entry)?.Text != "")
                        {
                            PhoneNumber p = new PhoneNumber()
                            {
                                Number = (s.Children[0] as Entry)?.Text,
                                PhoneNumberType_FK = (_phoneNumberTypes.ElementAt(((Picker) s.Children[1]).SelectedIndex)).ID,
                            };
                            pointOfSale.PhoneNumbers.Add(p);
                        }
                    }
                    if (((((LOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 1, OpenTime = LOpenTime.Time, CloseTime = LCloseTime.Time });
                    }
                    if (((((MeOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 2, OpenTime = MaOpenTime.Time, CloseTime = MeCloseTime.Time });
                    }
                    if (((((MaOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 3, OpenTime = MeOpenTime.Time, CloseTime = MaCloseTime.Time });
                    }
                    if (((((JOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 4, OpenTime = JOpenTime.Time, CloseTime = JCloseTime.Time });
                    }
                    if (((((VOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 5, OpenTime = VOpenTime.Time, CloseTime = VCloseTime.Time });
                    }
                    if (((((SOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 6, OpenTime = SOpenTime.Time, CloseTime = SCloseTime.Time });
                    }
                    if (((((DOpenTime.Parent as StackLayout)?.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 7, OpenTime = DOpenTime.Time, CloseTime = DCloseTime.Time });
                    }

                    try
                    {
                        pointOfSale = await pointOfSaleRc.PostAsync(pointOfSale);
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
                    if (pointOfSale == null)
                    {
                        await PopupNavigation.PopAsync();
                        await DisplayAlert("Erreur", "Erreur dans l'ajout du point de vente!", "Ok");
                    }
                    else
                    {
                        await PopupNavigation.PopAsync();
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    await PopupNavigation.PopAsync();
                    await DisplayAlert("Erreur", "Erreur dans l'ajout de l'address!", "Ok");
                }
            }
        }
    }
}
