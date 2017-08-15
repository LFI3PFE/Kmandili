using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;

namespace Kmandili.Views.Admin.Edit.EditCategories
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoriesList : ContentPage
	{
		public CategoriesList()
		{
			InitializeComponent ();
            Load();
		}

	    public async void Load()
	    {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var categoriesRestClient = new RestClient<Category>();
	        List.ItemsSource = await categoriesRestClient.GetAsync();
            ListLayout.IsVisible = true;
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
        }

	    private async void SelectedNot(object sender, ItemTappedEventArgs e)
	    {
	        List.SelectedItem = null;
	        await PopupNavigation.PushAsync(new EditCategoryPopupPage((e.Item as Category), this));
	    }


        private async void RemoveCategory(object sender, EventArgs e)
	    {
	        await DisplayAlert("yoo", "lqksd", "ok");
	    }

    }
}
