using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryPayments : PopupPage
	{
        private DeleveryList deleveryList;
        private DeleveryMethod deleveryMethod;

        public EditDeleveryPayments(DeleveryMethod deleveryMethod, DeleveryList deleveryList)
        {
            InitializeComponent();
            this.deleveryMethod = deleveryMethod;
            this.deleveryList = deleveryList;
        }

        private async void ComfirmTapped(object sender, EventArgs e)
        {
            
        }

        private async void DismissTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
    }
}
