using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddDeleveryPopupPage : PopupPage
	{
        private DeleveryList deleveryList;
        private List<DeleveryPaymentsViewModel> deleveryPaymentsViewModels = new List<DeleveryPaymentsViewModel>();

        private class DeleveryPaymentsViewModel
        {
            public Models.Payment payment { get; set; }
            public bool exist { get; set; }
        }

        public AddDeleveryPopupPage(DeleveryList deleveryList)
        {
            InitializeComponent();
            List.SeparatorVisibility = SeparatorVisibility.None;
            this.deleveryList = deleveryList;
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
                    payment.Orders.Clear();
                    payment.PastryDeleveryPayments.Clear();
                    payment.DeleveryMethods.Clear();
                    deleveryPaymentsViewModels.Add(new DeleveryPaymentsViewModel()
                    {
                        payment = payment,
                        exist = false
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
            if (string.IsNullOrEmpty(DeleveryName.Text))
            {
                await DisplayAlert("Erreur", "Un nom doit être fournit pour la nouvelle méthode de livraison", "Ok");
                return;
            }
            if (deleveryPaymentsViewModels.All(dpvm => !dpvm.exist))
            {
                await DisplayAlert("Erruer", "Au moins une méthode de paiement doit être séléctionner.", "Ok");
                return;
            }
            try
            {
                await PopupNavigation.PushAsync(new LoadingPopupPage());
                var newDelevery = new DeleveryMethod()
                {
                    DeleveryType = DeleveryName.Text,
                };
                foreach (var deleveryPaymentsViewModel in deleveryPaymentsViewModels)
                {
                    if (deleveryPaymentsViewModel.exist)
                    {
                        newDelevery.Payments.Add(deleveryPaymentsViewModel.payment);
                    }
                }
                var deleveryRC = new DeleveryMethodRestClient();
                if (await deleveryRC.PostAsync(newDelevery) == null)
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de l'insertion de la méthode de livraison, veuillez réessayer plus tard.",
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
            await PopupNavigation.PopAllAsync();
            deleveryList.Load();
        }

        private async void DismissTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private void SelectedNot(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            List.SelectedItem = null;
        }

	    private void Switch_OnToggled(object sender, EventArgs e)
	    {
	        
	    }

    }
}
