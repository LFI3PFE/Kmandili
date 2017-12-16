using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddDeleveryPopupPage
	{
        private readonly DeleveryList _deleveryList;
        private readonly List<DeleveryPaymentsViewModel> _deleveryPaymentsViewModels = new List<DeleveryPaymentsViewModel>();

        private class DeleveryPaymentsViewModel
        {
            public Models.Payment Payment { get; set; }
            public bool Exist { get; set; }
        }

        public AddDeleveryPopupPage(DeleveryList deleveryList)
        {
            InitializeComponent();
            List.SeparatorVisibility = SeparatorVisibility.None;
            _deleveryList = deleveryList;
            Load();
        }

        private async void Load()
        {
            var paymentRc = new RestClient<Models.Payment>();
            try
            {
                var payments = await paymentRc.GetAsync();
                _deleveryPaymentsViewModels.Clear();
                foreach (var payment in payments)
                {
                    payment.Orders.Clear();
                    payment.PastryDeleveryPayments.Clear();
                    payment.DeleveryMethods.Clear();
                    _deleveryPaymentsViewModels.Add(new DeleveryPaymentsViewModel()
                    {
                        Payment = payment,
                        Exist = false
                    });
                }
                List.HeightRequest = _deleveryPaymentsViewModels.Count * 40;
                List.ItemsSource = _deleveryPaymentsViewModels;
            }
            catch (HttpRequestException)
            {
                await PopupNavigation.PopAllAsync();
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
            }
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DeleveryName.Text))
            {
                await DisplayAlert("Erreur", "Un nom doit être fournit pour la nouvelle méthode de livraison", "Ok");
                return;
            }
            if (_deleveryPaymentsViewModels.All(dpvm => !dpvm.Exist))
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
                foreach (var deleveryPaymentsViewModel in _deleveryPaymentsViewModels)
                {
                    if (deleveryPaymentsViewModel.Exist)
                    {
                        newDelevery.Payments.Add(deleveryPaymentsViewModel.Payment);
                    }
                }
                var deleveryRc = new DeleveryMethodRestClient();
                if (await deleveryRc.PostAsync(newDelevery) == null)
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
            _deleveryList.Load();
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
