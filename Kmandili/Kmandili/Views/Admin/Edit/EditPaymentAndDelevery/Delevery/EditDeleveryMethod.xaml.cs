using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.Admin.Edit.EditCategories;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryMethod : PopupPage
	{
        private DeleveryList deleveryList;
        private DeleveryMethod deleveryMethod;

        public EditDeleveryMethod(DeleveryMethod deleveryMethod, DeleveryList deleveryList)
        {
            InitializeComponent();
            this.deleveryMethod = deleveryMethod;
            this.deleveryList = deleveryList;
            DeleveryName.Text = deleveryMethod.DeleveryType;
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
                            "Une erreur s'est produite lors de la mise à jour de la catégorie, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    deleveryList.Load();
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
