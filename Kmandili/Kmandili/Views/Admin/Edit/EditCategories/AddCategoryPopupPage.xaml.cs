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

namespace Kmandili.Views.Admin.Edit.EditCategories
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCategoryPopupPage : PopupPage
	{
	    private CategoriesList categoriesList;

		public AddCategoryPopupPage (CategoriesList categoriesList)
		{
			InitializeComponent ();
		    this.categoriesList = categoriesList;
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (!string.IsNullOrEmpty(CategoryName.Text))
	        {
	            var category = new Category()
	            {
	                CategoryName = CategoryName.Text
	            };
	            var categoryRC = new RestClient<Category>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (await categoryRC.PostAsync(category) == null)
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de l'ajout de la nouvelle catégorie, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    categoriesList.Load();
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
