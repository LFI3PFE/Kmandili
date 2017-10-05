using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryMethod : PopupPage
	{
        private DeleveryList deleveryList;
        private DeleveryMethod deleveryMethod;
	    private List<DeleveryPaymentsViewModel> deleveryPaymentsViewModels = new List<DeleveryPaymentsViewModel>();

        private class DeleveryPaymentsViewModel
        {
            public Models.Payment payment { get; set; }
            public bool exist { get; set; }
        }

        public EditDeleveryMethod(DeleveryMethod deleveryMethod, DeleveryList deleveryList)
        {
            InitializeComponent();
            this.deleveryMethod = deleveryMethod;
            this.deleveryList = deleveryList;
            DeleveryName.Text = deleveryMethod.DeleveryType;
            Load();
        }

	    private async void Load()
	    {
	        var paymentRC = new RestClient<Models.Payment>();
            try
            {
                var payments = await paymentRC.GetAsync();
                deleveryPaymentsViewModels.Clear();
                foreach (var payment in payments)
                {
                    deleveryPaymentsViewModels.Add(new DeleveryPaymentsViewModel()
                    {
                        payment = payment,
                        exist = deleveryMethod.Payments.Any(p => p.ID == payment.ID)
                    });
                }
                List.HeightRequest = deleveryPaymentsViewModels.Count * 40;
                List.ItemsSource = deleveryPaymentsViewModels;
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

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (DeleveryName.Text != deleveryMethod.DeleveryType)
            {
                var newDelevery = new DeleveryMethod()
                {
                    ID = deleveryMethod.ID,
                    DeleveryType = DeleveryName.Text,
                };
                var deleveryRC = new RestClient<DeleveryMethod>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (!(await deleveryRC.PutAsync(newDelevery.ID, newDelevery)))
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la méthode de livraison, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    await PopupNavigation.PopAsync();
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
            if (deleveryPaymentsViewModels.All(dpv => !dpv.exist))
            {
                await
                    DisplayAlert("Erreur",
                        "Au moins une méthode de paiement doit être séléctionner pour ce mode de livraison.", "Ok");
            }
            else
            {
                foreach (var deleveryPaymentsViewModel in deleveryPaymentsViewModels)
                {
                    if (deleveryMethod.Payments.All(dp => deleveryPaymentsViewModel.payment.ID != dp.ID) &&
                        deleveryPaymentsViewModel.exist)
                    {
                        deleveryMethod.Payments.Add(deleveryPaymentsViewModel.payment);
                    }
                    else if(deleveryMethod.Payments.Any(dp => deleveryPaymentsViewModel.payment.ID == dp.ID) &&
                        !deleveryPaymentsViewModel.exist)
                    {
                        deleveryMethod.Payments.Remove(
                            deleveryMethod.Payments.FirstOrDefault(p => p.ID == deleveryPaymentsViewModel.payment.ID));
                    }
                }
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    var deleveryRC = new DeleveryMethodRestClient();
                    foreach (var payment in deleveryMethod.Payments)
                    {
                        payment.Orders.Clear();
                        payment.PastryDeleveryPayments.Clear();
                        payment.DeleveryMethods.Clear();
                    }
                    var newDelevery = new DeleveryMethod()
                    {
                        ID = deleveryMethod.ID,
                        DeleveryType = deleveryMethod.DeleveryType,
                        Payments = deleveryMethod.Payments
                    };
                    if (!(await deleveryRC.PutAsyncPayments(newDelevery.ID, newDelevery)))
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la méthode de livraison, veuillez réessayer plus tard.",
                            "Ok");
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
                    return;
                }
            }
            deleveryList.Load();
            await PopupNavigation.PopAllAsync();
        }

        private async void DismissTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

	    private void SelectedNot(object sender, EventArgs e)
	    {
	        List.SelectedItem = null;
	    }
	}
}
