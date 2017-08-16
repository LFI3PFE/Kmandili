using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddDeleveryPopupPage : PopupPage
	{
		public AddDeleveryPopupPage ()
		{
			InitializeComponent ();
		}

	    private void ComfirmTapped(object sender, EventArgs e)
	    {
	        
	    }


        private async void DismissTapped(object sender, EventArgs e)
	    {
	        await PopupNavigation.PopAllAsync();
	    }

    }
}
