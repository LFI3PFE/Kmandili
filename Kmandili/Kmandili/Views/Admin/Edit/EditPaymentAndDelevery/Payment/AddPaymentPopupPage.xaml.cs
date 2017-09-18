using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPaymentPopupPage : PopupPage
	{
        private PaymentList paymentList;

        public AddPaymentPopupPage(PaymentList paymentList)
        {
            InitializeComponent();
            this.paymentList = paymentList;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MethodName.Text))
            {
                var payment = new Models.Payment()
                {
                    PaymentMethod = MethodName.Text
                };
                var paymentRC = new RestClient<Models.Payment>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (await paymentRC.PostAsync(payment) == null)
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de l'ajout de la nouvelle méthode de paiement, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    paymentList.Load();
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
