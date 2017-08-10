using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Kmandili.Views.PastryShopViews.Charts;
using Kmandili.Views.PastryShopViews.EditProfile;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kmandili.Views.PastryShopViews.OrderViewsAndFilter;

namespace Kmandili.Views.PastryShopViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopMasterPage : ContentPage
	{
	    private PastryShopMasterDetailPage pastryShopMasterDetailPage;
	    private PastryShop pastryShop;
		public PastryShopMasterPage (PastryShopMasterDetailPage pastryShopMasterDetailPage, PastryShop pastryShop)
		{
			InitializeComponent ();
		    this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
		    this.pastryShop = pastryShop;
            UpdateOrderNotificationNumber(pastryShop.Orders.ToList());
        }

	    protected async override void OnAppearing()
	    {
	        PastryShopRestClient pastryShopRC = new PastryShopRestClient();
	        try
	        {
                pastryShop = await pastryShopRC.GetAsyncById(pastryShop.ID);
            }
	        catch (HttpRequestException)
	        {
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                return;
	        }
	    }

	    public void Logout(object sender, EventArgs e)
        {
            App.Logout();
        }

        public async void UpdateOrderNotificationNumber()
        {
            PastryShopRestClient pastryShopRestClient = new PastryShopRestClient();
            try
            {
                pastryShop = await pastryShopRestClient.GetAsyncById(pastryShop.ID);
            }
            catch (HttpRequestException)
            {
                await
                    DisplayAlert("Erreur",
                        "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                        "Ok");
                return;
            }
            if (pastryShop == null) return;
            int number = pastryShop.Orders.Count(o => !o.SeenPastryShop);
            NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        public void UpdateOrderNotificationNumber(List<Order> orders)
        {
            int number = orders.Count(o => !o.SeenPastryShop);
            NorificationsNumber.Source = "_" + (number != 0 ? (number > 9 ? "9plus.png" : number + ".png") : "");
        }

        private async void ToOrderList(object sender, EventArgs e)
        {
            pastryShopMasterDetailPage.IsPresented = false;
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new PSOrderList(pastryShop.ID));
            //NavigationPage nav = new NavigationPage(new ContentPage());
            //await nav.PushAsync(new PastryShopOrderList());
            //await Navigation.PushModalAsync(nav);
        }

	    //private async void ToEditDeleveryMethod(object sender, EventArgs e)
	    //{
     //       pastryShopMasterDetailPage.IsPresented = false;
     //       pastryShopMasterDetailPage.hasNavigatedToEdit = true;
     //       await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new EditDeleveryMethods(pastryShopMasterDetailPage));
     //   }


        private async void ToEditProfile(object sender, EventArgs e)
	    {
            pastryShopMasterDetailPage.IsPresented = false;
            pastryShopMasterDetailPage.hasNavigatedToEdit = true;
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(new EditProfileInfo(pastryShop.ID, true));
        }

	    private async void ToChart(object sender, EventArgs e)
	    {
            pastryShopMasterDetailPage.IsPresented = false;
	        var doughnutPage = new Doughnut()
	        {
                Icon = "",
                Title = "Meilleurs produits"
	        };
	        var linePage = new Line(pastryShop)
	        {
	            Icon = "",
	            Title = "Graphe des commandes"
	        };
            TabbedPage chartsPage = new TabbedPage()
            {
                Children =
                {
                    linePage,
                    doughnutPage,
                }
            };
            await pastryShopMasterDetailPage.Detail.Navigation.PushAsync(chartsPage);
        }
    }
}
