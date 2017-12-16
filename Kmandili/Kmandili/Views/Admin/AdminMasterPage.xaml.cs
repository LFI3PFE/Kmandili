using System;
using Kmandili.Views.Admin.Charts;
using Kmandili.Views.Admin.Edit;
using Kmandili.Views.Admin.Edit.EditCategories;
using Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Delevery;
using Kmandili.Views.Admin.Edit.EditPaymentAndDelevery.Payment;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdminMasterPage
	{
	    private readonly AdminMasterDetailPage _adminMasterDetailPage;

		public AdminMasterPage (AdminMasterDetailPage adminMasterDetailPage)
		{
			InitializeComponent ();
		    _adminMasterDetailPage = adminMasterDetailPage;
		}

	    private void Logout(object sender, EventArgs e)
	    {
	        App.Logout();
	    }

	    private async void ToChart(object sender, EventArgs e)
	    {
            _adminMasterDetailPage.IsPresented = false;
            var newUsersChart = new NewUserChart()
            {
                Icon = "",
                Title = "Nouveaux Utilisateurs"
            };
            var usersChart = new UsersChart()
            {
                Icon = "",
                Title = "Evolutions des Utilisateurs"
            };
            TabbedPage chartsPage = new TabbedPage()
            {
                Children =
                {
                    newUsersChart,
                    usersChart,
                },
            };
            await Navigation.PushAsync(chartsPage);
	    }

	    private async void ManageCategories_OnTapped(object sender, EventArgs e)
	    {
	        _adminMasterDetailPage.IsPresented = false;
	        await Navigation.PushAsync(new CategoriesList());
	    }

	    private async void ManageDelevery_OnTapped(object sender, EventArgs e)
	    {
	        _adminMasterDetailPage.IsPresented = false;
	        var tabbedPage = new TabbedPage()
	        {
                Children =
                {
                    new DeleveryList()
                    {
                        Title = "Méthodes de livraison"
                    },
                    new PaymentList()
                    {
                        Title = "Méthodes de paiement"
                    }
                }
            };
	        await Navigation.PushAsync(tabbedPage);
	    }

	    private async void UpdateAdmin_OnTapped(object sender, EventArgs e)
	    {
	        _adminMasterDetailPage.IsPresented = false;
	        await Navigation.PushAsync(new EditAdmin());
	    }

    }
}
