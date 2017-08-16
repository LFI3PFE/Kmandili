using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
	    private ToolbarItem addToolbarItem;

		public CategoriesList()
		{
			InitializeComponent ();
            addToolbarItem = new ToolbarItem()
            {
                Icon = "plus.png",
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            addToolbarItem.Clicked += AddToolbarItem_Clicked;
		    ToolbarItems.Add(addToolbarItem);
            Load();
		}

        private async void AddToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new AddCategoryPopupPage(this));
        }

        public async void Load()
	    {
            ListLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            var categoriesRestClient = new RestClient<Category>();
            try
            {
                List.ItemsSource = await categoriesRestClient.GetAsync();
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
            var choix = await DisplayAlert("Confirmation", "Etes vous sure de vouloire supprimer cette catégorie?", "Oui", "Annuler");
            if (!choix) return;
            var id = (Int32.Parse((((sender as Image).Parent as StackLayout).Children[0] as Label).Text));
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            var categoryRC = new RestClient<Category>();
            var cat = await categoryRC.GetAsyncById(id);
            if (cat.PastryShops.Any() ||
                cat.Products.Any())
            {
                await PopupNavigation.PopAllAsync();
                await
                        DisplayAlert("Erreur",
                            "Impossible de supprimer cette catégorie, une ou plusieurs pâtisseries et / ou produits l'utilisent",
                            "Ok");
                return;
            }
            try
            {
                if (!(await categoryRC.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la catégorie, veuillez réessayer plus tard.",
                            "Ok");
                    return;
                }
                Load();
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
	    }

    }
}
