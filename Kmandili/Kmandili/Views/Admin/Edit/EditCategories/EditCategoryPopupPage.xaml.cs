using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditCategories
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCategoryPopupPage : PopupPage
	{
	    private CategoriesList categoriesList;
	    private Category category;

		public EditCategoryPopupPage (Category category, CategoriesList categoriesList)
		{
			InitializeComponent ();
		    this.category = category;
		    this.categoriesList = categoriesList;
		    CategoryName.Text = category.CategoryName;
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (CategoryName.Text != category.CategoryName)
	        {
	            var newCategory = new Category()
	            {
	                ID = category.ID,
	                CategoryName = CategoryName.Text,
	            };
	            var categoryRC = new RestClient<Category>();
	            try
	            {
	                await PopupNavigation.PushAsync(new LoadingPopupPage());
	                if (!(await categoryRC.PutAsync(newCategory.ID, newCategory)))
	                {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la catégorie, veuillez réessayer plus tard.",
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
