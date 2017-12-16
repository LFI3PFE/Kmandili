using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPayment
	{
        private readonly PaymentList _paymentList;
        private readonly Models.Payment _paymentMethod;

        public EditPayment(Models.Payment paymentMethod, PaymentList paymentList)
        {
            InitializeComponent();
            _paymentMethod = paymentMethod;
            _paymentList = paymentList;
            PaymentName.Text = paymentMethod.PaymentMethod;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (PaymentName.Text != _paymentMethod.PaymentMethod)
            {
                var newPayement = new Models.Payment()
                {
                    ID = _paymentMethod.ID,
                    PaymentMethod = PaymentName.Text,
                };
                var paymentRc = new RestClient<Models.Payment>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (!(await paymentRc.PutAsync(newPayement.ID, newPayement)))
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la méthode de paiement, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    _paymentList.Load();
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
            await PopupNavigation.PopAllAsync();
        }

        private async void DismissTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
    }
}
