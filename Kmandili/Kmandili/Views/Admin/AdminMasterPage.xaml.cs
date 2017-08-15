using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Views.Admin.Charts;
using Kmandili.Views.Admin.Edit.EditCategories;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdminMasterPage : ContentPage
	{
	    private AdminMasterDetailPage adminMasterDetailPage;

		public AdminMasterPage (AdminMasterDetailPage adminMasterDetailPage)
		{
			InitializeComponent ();
		    this.adminMasterDetailPage = adminMasterDetailPage;
		}

	    private void Logout(object sender, EventArgs e)
	    {
	        App.Logout();
	    }

	    private async void ToChart(object sender, EventArgs e)
	    {
            adminMasterDetailPage.IsPresented = false;
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
	        adminMasterDetailPage.IsPresented = false;
	        await Navigation.PushAsync(new CategoriesList());
	    }

    }
}
