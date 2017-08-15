using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
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

	    protected override System.Threading.Tasks.Task OnAppearingAnimationEnd()
	    {
            CategoryName.Focus();
            return base.OnAppearingAnimationEnd();
	    }

	    private async void ComfirmTapped(object sender, EventArgs e)
	    {
	        categoriesList.Load();
	        await PopupNavigation.PopAsync();
	    }

	    private async void DismissTapped(object sender, EventArgs e)
	    {
	        await PopupNavigation.PopAsync();
	    }

    }
}
