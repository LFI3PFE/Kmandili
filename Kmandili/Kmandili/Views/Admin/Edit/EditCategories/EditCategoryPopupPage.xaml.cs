using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditCategories
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCategoryPopupPage
	{
	    private readonly CategoriesList _categoriesList;
	    private readonly Category _category;

		public EditCategoryPopupPage (Category category, CategoriesList categoriesList)
		{
			InitializeComponent ();
		    _category = category;
		    _categoriesList = categoriesList;
		    CategoryName.Text = category.CategoryName;
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (CategoryName.Text != _category.CategoryName)
	        {
	            var newCategory = new Category()
	            {
	                ID = _category.ID,
	                CategoryName = CategoryName.Text,
	            };
	            var categoryRc = new RestClient<Category>();
	            try
	            {
	                await PopupNavigation.PushAsync(new LoadingPopupPage());
	                if (!(await categoryRc.PutAsync(newCategory.ID, newCategory)))
	                {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la mise à jour de la catégorie, veuillez réessayer plus tard.",
                            "Ok");
                        return;
                    }
                    _categoriesList.Load();
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
