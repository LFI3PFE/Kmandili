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
	public partial class PastryShopPointOfSaleForm : ContentPage
	{
        private PastryShop pastryShop;
        private List<Parking> parkings = new List<Parking>();
        private ObservableCollection<StackLayout> PhoneNumberStackLayouts = new ObservableCollection<StackLayout>();
        private List<PhoneNumberType> phoneNumberTypes = new List<PhoneNumberType>();
        private PastryShopEnteringPointOfSales pastryShopEnteringPointOfSales;
        public PastryShopPointOfSaleForm(PastryShopEnteringPointOfSales pastryShopEnteringPointOfSales, PastryShop pastryShop)
        {
            InitializeComponent();
            this.pastryShop = pastryShop;
            this.pastryShopEnteringPointOfSales = pastryShopEnteringPointOfSales;
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
            CreationDate.MaximumDate = DateTime.Now.Date;
            RestClient<Parking> parkingRC = new RestClient<Parking>();
            try
            {
                parkings = await parkingRC.GetAsync();
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
            if (parkings == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
            ParkingPicker.ItemsSource = parkings;
            ParkingPicker.SelectedIndex = 0;

            RestClient<PhoneNumberType> phoneNumberTypeRC = new RestClient<PhoneNumberType>();
            try
            {
                phoneNumberTypes = await phoneNumberTypeRC.GetAsync();
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
            if (phoneNumberTypes == null)
            {
                await PopupNavigation.PopAsync();
                return;
            }
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

        private async Task<bool> validAddress()
        {
            int x;
            if (Number.Text == null || Number.Text == "")
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment est obligateur!", "Ok");
                return false;
            }
            else if (!int.TryParse(Number.Text, out x))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Numéro De Bâtiment ne doit contenir que des Chiffres!", "Ok");
                Number.Text = "";
                return false;
            }
            else if (Street.Text == null || Street.Text == "")
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Rue est obligateur!", "Ok");
                return false;
            }
            else if (City.Text == null || City.Text == "")
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Ville est obligateur!", "Ok");
                return false;
            }
            else if (ZipCode.Text == null || ZipCode.Text == "")
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
            else if (!int.TryParse(ZipCode.Text, out x))
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Co. Postal ne doit contenir que des Chiffres!", "Ok");
                ZipCode.Text = "";
                return false;
            }
            else if (State.Text == null || State.Text == "")
            {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Le champ Gouvernorat est obligateur!", "Ok");
                return false;
            }
            else if (Country.Text == null || Country.Text == "")
            {
                await PopupNavigation.PopAllAsync();
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
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Le champ Numero de Telephone N°" + (PhoneNumberStackLayouts.IndexOf(s) + 1) + " ne doit contenir que des chiffres!", "Ok");
                        (s.Children[0] as Entry).Text = "";
                        return false;
                    }
                    else if (phoneNumber.Length != 8)
                    {
                        await PopupNavigation.PopAllAsync();
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
                await PopupNavigation.PopAllAsync();
            }
            return exist;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (await validAddress() && await validPhoneNumber())
            {
                RestClient<PointOfSale> pointOfSaleRC = new RestClient<PointOfSale>();
                RestClient<Address> addressRC = new RestClient<Address>();
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
                    address = await addressRC.PostAsync(address);
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
                        ParkingType_FK = parkings.ElementAt(ParkingPicker.SelectedIndex).ID,
                        PastryShop_FK = pastryShop.ID,
                        Address_FK = address.ID,
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
                            pointOfSale.PhoneNumbers.Add(p);
                        }
                    }
                    if (((((LOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 1, OpenTime = LOpenTime.Time, CloseTime = LCloseTime.Time });
                    }
                    if (((((MeOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 2, OpenTime = MaOpenTime.Time, CloseTime = MeCloseTime.Time });
                    }
                    if (((((MaOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 3, OpenTime = MeOpenTime.Time, CloseTime = MaCloseTime.Time });
                    }
                    if (((((JOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 4, OpenTime = JOpenTime.Time, CloseTime = JCloseTime.Time });
                    }
                    if (((((VOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 5, OpenTime = VOpenTime.Time, CloseTime = VCloseTime.Time });
                    }
                    if (((((SOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 6, OpenTime = SOpenTime.Time, CloseTime = SCloseTime.Time });
                    }
                    if (((((DOpenTime.Parent as StackLayout).Parent as StackLayout).Parent as Grid).Parent as StackLayout).ClassId == "1")
                    {
                        pointOfSale.WorkDays.Add(new WorkDay() { Day = 7, OpenTime = DOpenTime.Time, CloseTime = DCloseTime.Time });
                    }

                    try
                    {
                        pointOfSale = await pointOfSaleRC.PostAsync(pointOfSale);
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
                        return;
                    }
                    else
                    {
                        //pastryShopEnteringPointOfSales.load();
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
