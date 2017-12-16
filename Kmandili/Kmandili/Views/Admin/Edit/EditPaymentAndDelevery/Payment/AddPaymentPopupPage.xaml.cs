using System;
using System.Net.Http;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPaymentPopupPage
	{
        private readonly PaymentList _paymentList;

        public AddPaymentPopupPage(PaymentList paymentList)
        {
            InitializeComponent();
            _paymentList = paymentList;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MethodName.Text))
            {
                var payment = new Models.Payment()
                {
                    PaymentMethod = MethodName.Text
                };
                var paymentRc = new RestClient<Models.Payment>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (await paymentRc.PostAsync(payment) == null)
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de l'ajout de la nouvelle méthode de paiement, veuillez réessayer plus tard.",
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
