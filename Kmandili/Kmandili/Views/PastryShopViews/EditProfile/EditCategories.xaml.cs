using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCategories : PopupPage
	{
	    private List<Category> categories; 
        private List<Category> newSelectedCategories = new List<Category>();
        private List<Category> toRemoveCategories = new List<Category>();

	    private PastryShop pastryShop;

		public EditCategories (PastryShop pastryShop)
		{
            BackgroundColor = Color.FromHex("#CC000000");
            this.pastryShop = pastryShop;
			InitializeComponent ();
		    Load();
		}

	    private async void Load()
	    {
	        var categoryRC = new RestClient<Category>();
	        categories = await categoryRC.GetAsync();
            CategoriesLayout.Children.Clear();
            categories.ForEach(c => CategoriesLayout.Children.Add(MakeCategoryLayout(c)));
	    }

	    private StackLayout MakeCategoryLayout(Category category)
	    {
            var categoryLayout = new StackLayout();
            Grid categoryGrid = new Grid();
            categoryGrid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
	        categoryGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(3, GridUnitType.Star)});
            categoryGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Switch categorySwitch = new Switch()
            {
                ClassId = category.ID.ToString(),
                IsToggled = pastryShop.Categories.Any(c => c.ID == category.ID)
            };
            categorySwitch.Toggled += CategorySwitch_Toggled;
	        categoryGrid.Children.Add(new Label()
	        {
	            Text = category.CategoryName,
	            FontSize = 18,
	            TextColor = Color.Black,
	        }, 0, 0);
            categoryGrid.Children.Add(categorySwitch, 1, 0);
            categoryLayout.Children.Add(categoryGrid);
	        return categoryLayout;
	    }

        private void CategorySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var senderSwitch = (sender as Switch);
            if (senderSwitch == null) return;
            if (senderSwitch.IsToggled)
            {
                if (pastryShop.Categories.Any(c => c.ID == Int32.Parse(senderSwitch.ClassId)))
                {
                    toRemoveCategories.Remove(toRemoveCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
                else
                {
                    newSelectedCategories.Add(categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
            }
            else
            {
                if (pastryShop.Categories.Any(c => c.ID == Int32.Parse(senderSwitch.ClassId)))
                {
                    toRemoveCategories.Add(pastryShop.Categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
                else
                {
                    newSelectedCategories.Remove(newSelectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
            }
        }

	    private async void Dismiss(object sender, EventArgs e)
	    {
	        await PopupNavigation.PopAsync();
	    }

	    private async void Apply(object sender, EventArgs e)
	    {
	        
	    }
    }
}
