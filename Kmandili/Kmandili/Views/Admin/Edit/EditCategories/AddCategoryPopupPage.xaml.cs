using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin.Edit.EditCategories
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCategoryPopupPage
	{
	    private readonly CategoriesList _categoriesList;

		public AddCategoryPopupPage (CategoriesList categoriesList)
		{
			InitializeComponent ();
		    _categoriesList = categoriesList;
		}

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        if (!string.IsNullOrEmpty(CategoryName.Text))
	        {
	            var category = new Category()
	            {
	                CategoryName = CategoryName.Text
	            };
	            var categoryRc = new RestClient<Category>();
                try
                {
                    await PopupNavigation.PushAsync(new LoadingPopupPage());
                    if (await categoryRc.PostAsync(category) == null)
                    {
                        await PopupNavigation.PopAllAsync();
                        await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de l'ajout de la nouvelle catégorie, veuillez réessayer plus tard.",
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
