using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPayment : PopupPage
	{
        private PaymentList paymentList;
        private Models.Payment paymentMethod;

        public EditPayment(Models.Payment paymentMethod, PaymentList paymentList)
        {
            InitializeComponent();
            this.paymentMethod = paymentMethod;
            this.paymentList = paymentList;
            PaymentName.Text = paymentMethod.PaymentMethod;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (PaymentName.Text != paymentMethod.PaymentMethod)
            {
                var newPayement = new Models.Payment()
                {
                    ID = paymentMethod.ID,
                    PaymentMethod = PaymentName.Text,
                };
                var paymentRC = new RestClient<Models.Payment>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (!(await paymentRC.PutAsync(newPayement.ID, newPayement)))
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la catégorie, veuillez réessayer plus tard.",
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
