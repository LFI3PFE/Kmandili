using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PEditAddDeleveryMethodForm
	{
	    private readonly EditDeleveryMethods _editDeleveryMethods;
	    private List<DeleveryMethod> _deleveryMethods;
	    private List<DeleveryDelay> _deleveryDelays;
	    private readonly List<Payment> _selectedPayments = new List<Payment>();
	    private readonly PastryShop _pastryShop;

		public PEditAddDeleveryMethodForm(EditDeleveryMethods editDeleveryMethods, PastryShop pastryShop)
		{
            BackgroundColor = Color.FromHex("#CC000000");
		    _pastryShop = pastryShop;
		    _editDeleveryMethods = editDeleveryMethods;
            InitializeComponent ();
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
            var deleveryMethodRc = new RestClient<DeleveryMethod>();
            var deleveryDelayRc = new RestClient<DeleveryDelay>();

            try
            {
                _deleveryMethods = await deleveryMethodRc.GetAsync();
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
                return;
            }
            DeleveryPicker.ItemsSource = _deleveryMethods = _deleveryMethods.Where(d => _pastryShop.PastryShopDeleveryMethods.All(pdm => pdm.DeleveryMethod_FK != d.ID)).ToList();
	        DeleveryPicker.SelectedIndex = 0;
            try
            {
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
            if (_deleveryDelays == null)
            {
                return;
            }
            DelayPicker.ItemsSource = _deleveryDelays;
            DelayPicker.SelectedIndex = 0;
        }

	    private StackLayout MakePaymentLayout(Payment payment)
	    {
            StackLayout mainPaymentLayout = new StackLayout() {Orientation = StackOrientation.Horizontal, Spacing = 20};
            Grid paymentGrid = new Grid();
            paymentGrid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
            paymentGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(4, GridUnitType.Star)});
            paymentGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            Switch paymentSwitch = new Switch() {ClassId = payment.ID.ToString()};
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
                .Payments.FirstOrDefault(p => p.ID == Int32.Parse((sender as Switch)?.ClassId));
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
	                DisplayAlert("Erreur", "Vous devez selection au moin une methode de payment pour ce type de livraison!",
	                    "Ok");
                return;
	        }
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        var selectedDeleveryMethod = (DeleveryPicker.SelectedItem as DeleveryMethod);
	        var selectedDelay = ((DeleveryDelay) DelayPicker.SelectedItem);
	        if (selectedDelay != null && selectedDeleveryMethod != null)
	        {
	            var pastryShopDeleveryMethod = new PastryShopDeleveryMethod()
	            {
	                PastryShop_FK = _pastryShop.ID,
	                DeleveryDelay_FK = selectedDelay.ID,
	                DeleveryMethod_FK = selectedDeleveryMethod.ID
	            };
	            var pastryShopDeleveryMethodRc = new RestClient<PastryShopDeleveryMethod>();
	            try
	            {
	                pastryShopDeleveryMethod = await pastryShopDeleveryMethodRc.PostAsync(pastryShopDeleveryMethod);
	            }
	            catch (HttpRequestException)
	            {
	                await PopupNavigation.PopAllAsync();
	                await
	                    DisplayAlert("Erreur",
	                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
	                        "Ok");
	                return;
	            }
	            if(pastryShopDeleveryMethod == null) { await PopupNavigation.PopAsync(); return;}
	            var pastryDeleveryPaymentRc = new RestClient<PastryDeleveryPayment>();
	            foreach (var selectedPayment in _selectedPayments)
	            {
	                var pastryDeleveryPayment = new PastryDeleveryPayment()
	                {
	                    PastryShopDeleveryMethod_FK = pastryShopDeleveryMethod.ID,
	                    Payment_FK = selectedPayment.ID
	                };
	                try
	                {
	                    if (await pastryDeleveryPaymentRc.PostAsync(pastryDeleveryPayment) == null) return;
	                }
	                catch (HttpRequestException)
	                {
	                    await PopupNavigation.PopAllAsync();
	                    await
	                        DisplayAlert("Erreur",
	                            "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
	                            "Ok");
	                    return;
	                }
	            }
	        }
	        await PopupNavigation.PopAsync();
	        _editDeleveryMethods.Load(true);
	    }
    }
}
