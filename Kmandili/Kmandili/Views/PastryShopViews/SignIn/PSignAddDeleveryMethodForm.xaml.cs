using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PSignAddDeleveryMethodForm
	{
        private readonly PastryShopThirdStep _editDeleveryMethods;
        private List<DeleveryMethod> _deleveryMethods;
        private List<DeleveryDelay> _deleveryDelays;
        private readonly List<Payment> _selectedPayments = new List<Payment>();
        private readonly PastryShop _pastryShop;

        public PSignAddDeleveryMethodForm(PastryShopThirdStep editDeleveryMethods, PastryShop pastryShop)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            _pastryShop = pastryShop;
            _editDeleveryMethods = editDeleveryMethods;
            InitializeComponent();
            Load();
            DeleveryPicker.SelectedIndexChanged += DeleveryPicker_SelectedIndexChanged;
        }

        private void DeleveryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var senderPicker = (sender as Picker);
            if (senderPicker?.SelectedItem == null) return;
            _selectedPayments.Clear();
            PaymentsLayout.Children.Clear();
            (senderPicker.SelectedItem as DeleveryMethod)?.Payments.ToList().ForEach(p => PaymentsLayout.Children.Add(MakePaymentLayout(p)));
        }

        private async void Load()
        {
            PaymentsLayout.IsVisible = false;
            DeleveryPicker.IsVisible = false;
            DelayPicker.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;

            var deleveryMethodRc = new RestClient<DeleveryMethod>();
            var deleveryDelayRc = new RestClient<DeleveryDelay>();

            try
            {
                _deleveryMethods = await deleveryMethodRc.GetAsync();
                _deleveryDelays = await deleveryDelayRc.GetAsync();
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
            if (_deleveryMethods == null)
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            DeleveryPicker.ItemsSource = _deleveryMethods = _deleveryMethods.Where(d => _pastryShop.PastryShopDeleveryMethods.All(pdm => pdm.DeleveryMethod.ID != d.ID)).ToList();
            DeleveryPicker.SelectedIndex = 0;
            if (_deleveryDelays == null)
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            DelayPicker.ItemsSource = _deleveryDelays;
            DelayPicker.SelectedIndex = 0;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            DeleveryPicker.IsVisible = true;
            DelayPicker.IsVisible = true;
            PaymentsLayout.IsVisible = true;
        }

        private StackLayout MakePaymentLayout(Payment payment)
        {
            StackLayout mainPaymentLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20 };
            Grid paymentGrid = new Grid();
            paymentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            paymentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) });
            paymentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            Switch paymentSwitch = new Switch() { ClassId = payment.ID.ToString() };
            paymentSwitch.Toggled += PaymentSwitch_Toggled;
            paymentGrid.Children.Add(new Label()
            {
                Text = payment.PaymentMethod,
                FontSize = 18,
                TextColor = Color.Black
            }, 0, 0);
            paymentGrid.Children.Add(paymentSwitch, 1, 0);
            mainPaymentLayout.Children.Add(paymentGrid);
            return mainPaymentLayout;
        }

        private void PaymentSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var sendSwitch = (sender as Switch);
            var payment = _deleveryMethods.ElementAt(DeleveryPicker.SelectedIndex)
                .Payments.FirstOrDefault(p => p.ID == Int32.Parse((sender as Switch)?.ClassId ?? throw new InvalidOperationException()));
            if (sendSwitch != null && sendSwitch.IsToggled)
            {
                _selectedPayments.Add(payment);
            }
            else
            {
                _selectedPayments.Remove(payment);
            }
        }

        private async void Dismiss(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private async void Add(object sender, EventArgs e)
        {
            if (_selectedPayments.Count == 0)
            {
                await
                    DisplayAlert("Erreur", "Vous devez selection au moin une méthode de payment pour ce type de livraison!",
                        "Ok");
                return;
            }
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var selectedDeleveryMethod = (DeleveryPicker.SelectedItem as DeleveryMethod);
            var selectedDelay = (DelayPicker.SelectedItem as DeleveryDelay);
            var pastryShopDeleveryMethod = new PastryShopDeleveryMethod()
            {
                DeleveryMethod = selectedDeleveryMethod,
                DeleveryDelay = selectedDelay,
            };
            foreach (var selectedPayment in _selectedPayments)
            {
                pastryShopDeleveryMethod.PastryDeleveryPayments.Add(new PastryDeleveryPayment()
                {
                    PastryShopDeleveryMethod = pastryShopDeleveryMethod,
                    Payment = selectedPayment
                });
            }
            _pastryShop.PastryShopDeleveryMethods.Add(pastryShopDeleveryMethod);
            await PopupNavigation.PopAllAsync();
            _editDeleveryMethods.RefreshDeleveryMethods();
        }
    }
}
