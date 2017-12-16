using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryMethod
	{
        private readonly DeleveryList _deleveryList;
        private readonly DeleveryMethod _deleveryMethod;
	    private readonly List<DeleveryPaymentsViewModel> _deleveryPaymentsViewModels = new List<DeleveryPaymentsViewModel>();

        private class DeleveryPaymentsViewModel
        {
            public Models.Payment Payment { get; set; }
            public bool Exist { get; set; }
        }

        public EditDeleveryMethod(DeleveryMethod deleveryMethod, DeleveryList deleveryList)
        {
            InitializeComponent();
            _deleveryMethod = deleveryMethod;
            _deleveryList = deleveryList;
            DeleveryName.Text = deleveryMethod.DeleveryType;
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
                    _deleveryPaymentsViewModels.Add(new DeleveryPaymentsViewModel()
                    {
                        Payment = payment,
                        Exist = _deleveryMethod.Payments.Any(p => p.ID == payment.ID)
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
            if (DeleveryName.Text != _deleveryMethod.DeleveryType)
            {
                var newDelevery = new DeleveryMethod()
                {
                    ID = _deleveryMethod.ID,
                    DeleveryType = DeleveryName.Text,
                };
                var deleveryRc = new RestClient<DeleveryMethod>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (!(await deleveryRc.PutAsync(newDelevery.ID, newDelevery)))
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
            if (_deleveryPaymentsViewModels.All(dpv => !dpv.Exist))
            {
                await
                    DisplayAlert("Erreur",
                        "Au moins une méthode de paiement doit être séléctionner pour ce mode de livraison.", "Ok");
            }
            else
            {
                foreach (var deleveryPaymentsViewModel in _deleveryPaymentsViewModels)
                {
                    if (_deleveryMethod.Payments.All(dp => deleveryPaymentsViewModel.Payment.ID != dp.ID) &&
                        deleveryPaymentsViewModel.Exist)
                    {
                        _deleveryMethod.Payments.Add(deleveryPaymentsViewModel.Payment);
                    }
                    else if(_deleveryMethod.Payments.Any(dp => deleveryPaymentsViewModel.Payment.ID == dp.ID) &&
                        !deleveryPaymentsViewModel.Exist)
                    {
                        _deleveryMethod.Payments.Remove(
                            _deleveryMethod.Payments.FirstOrDefault(p => p.ID == deleveryPaymentsViewModel.Payment.ID));
                    }
                }
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    var deleveryRc = new DeleveryMethodRestClient();
                    foreach (var payment in _deleveryMethod.Payments)
                    {
                        payment.Orders.Clear();
                        payment.PastryDeleveryPayments.Clear();
                        payment.DeleveryMethods.Clear();
                    }
                    var newDelevery = new DeleveryMethod()
                    {
                        ID = _deleveryMethod.ID,
                        DeleveryType = _deleveryMethod.DeleveryType,
                        Payments = _deleveryMethod.Payments
                    };
                    if (!(await deleveryRc.PutAsyncPayments(newDelevery.ID, newDelevery)))
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
            _deleveryList.Load();
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
