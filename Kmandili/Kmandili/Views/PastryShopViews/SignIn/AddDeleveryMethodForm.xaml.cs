using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.EditProfile;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddDeleveryMethodForm : PopupPage
	{
        private PastryShopThirdStep editDeleveryMethods;
        private List<DeleveryMethod> deleveryMethods;
        private List<DeleveryDelay> deleveryDelays;
        private List<Payment> selectedPayments = new List<Payment>();
        private PastryShop pastryShop;

        public AddDeleveryMethodForm(PastryShopThirdStep editDeleveryMethods, PastryShop pastryShop)
        {
            BackgroundColor = Color.FromHex("#CC000000");
            this.pastryShop = pastryShop;
            this.editDeleveryMethods = editDeleveryMethods;
            InitializeComponent();
            Load();
            DeleveryPicker.SelectedIndexChanged += DeleveryPicker_SelectedIndexChanged;
        }

        private void DeleveryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var senderPicker = (sender as Picker);
            if (senderPicker?.SelectedItem == null) return;
            selectedPayments.Clear();
            PaymentsLayout.Children.Clear();
            (senderPicker.SelectedItem as DeleveryMethod)?.Payments.ToList().ForEach(p => PaymentsLayout.Children.Add(MakePaymentLayout(p)));
        }

        private async void Load()
        {
            //ContentLayout.IsVisible = false;
            PaymentsLayout.IsVisible = false;
            DeleveryPicker.IsVisible = false;
            DelayPicker.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;

            var deleveryMethodRC = new RestClient<DeleveryMethod>();
            var deleveryDelayRC = new RestClient<DeleveryDelay>();

            deleveryMethods = await deleveryMethodRC.GetAsync();
            if (deleveryMethods == null)
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            DeleveryPicker.ItemsSource = deleveryMethods = deleveryMethods.Where(d => pastryShop.PastryShopDeleveryMethods.All(pdm => pdm.DeleveryMethod.ID != d.ID)).ToList();
            DeleveryPicker.SelectedIndex = 0;
            deleveryDelays = await deleveryDelayRC.GetAsync();
            if (deleveryDelays == null)
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            DelayPicker.ItemsSource = deleveryDelays;
            DelayPicker.SelectedIndex = 0;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            //ContentLayout.IsVisible = true;
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
            var payment = deleveryMethods.ElementAt(DeleveryPicker.SelectedIndex)
                .Payments.FirstOrDefault(p => p.ID == Int32.Parse((sender as Switch)?.ClassId));
            if (sendSwitch != null && sendSwitch.IsToggled)
            {
                selectedPayments.Add(payment);
            }
            else
            {
                selectedPayments.Remove(payment);
            }
        }

        private async void Dismiss(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private async void Add(object sender, EventArgs e)
        {
            if (selectedPayments.Count == 0)
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
            foreach (var selectedPayment in selectedPayments)
            {
                pastryShopDeleveryMethod.PastryDeleveryPayments.Add(new PastryDeleveryPayment()
                {
                    PastryShopDeleveryMethod = pastryShopDeleveryMethod,
                    Payment = selectedPayment
                });
            }
            pastryShop.PastryShopDeleveryMethods.Add(pastryShopDeleveryMethod);
            await PopupNavigation.PopAllAsync();
            editDeleveryMethods.RefreshDeleveryMethods();
        }
    }
}
