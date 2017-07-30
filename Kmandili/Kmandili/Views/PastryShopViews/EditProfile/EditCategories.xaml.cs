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

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCategories : PopupPage
	{
	    private List<Category> categories; 
        private List<Category> newSelectedCategories = new List<Category>();
        private List<Category> toRemoveCategories = new List<Category>();
        private List<Category> allSelectedCategories = new List<Category>();

	    private EditProfileInfo editProfileInfo;

	    private PastryShop pastryShop;

		public EditCategories (EditProfileInfo editProfileInfo, PastryShop pastryShop)
		{
            BackgroundColor = Color.FromHex("#CC000000");
            this.pastryShop = pastryShop;
		    this.editProfileInfo = editProfileInfo;
			InitializeComponent ();
		    Load();
		}

	    private async void Load()
	    {
            pastryShop.Categories.ToList().ForEach(c => allSelectedCategories.Add(c));
	        var categoryRC = new RestClient<Category>();
	        try
	        {
                categories = await categoryRC.GetAsync();
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
	        if (categories == null) return;
            CategoriesLayout.Children.Clear();
            categories.ForEach(c => CategoriesLayout.Children.Add(MakeCategoryLayout(c)));
	    }

	    private StackLayout MakeCategoryLayout(Category category)
	    {
            var categoryLayout = new StackLayout();
            Grid categoryGrid = new Grid();
            categoryGrid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
	        categoryGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(4, GridUnitType.Star)});
            categoryGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
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
                allSelectedCategories.Add(categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
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
                allSelectedCategories.Remove(allSelectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
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
	        if (!newSelectedCategories.Any() && !toRemoveCategories.Any())
	        {
	            await PopupNavigation.PopAsync();
                return;
	        }
	        if (!allSelectedCategories.Any())
	        {
	            await DisplayAlert("Erreur", "Au moins une catégorie doit être selectionné!", "Ok");
                return;
	        }
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        foreach (var category in pastryShop.Categories)
	        {
                category.PastryShops.Clear();
                category.Products.Clear();
            }
	        foreach (var newSelectedCategory in newSelectedCategories)
	        {
	            newSelectedCategory.PastryShops.Clear();
                newSelectedCategory.Products.Clear();
            }
            PastryShop p = new PastryShop()
	        {
                ID = pastryShop.ID,
	            Name = pastryShop.Name,
                Email = pastryShop.Email,
                Password = pastryShop.Password,
                ShortDesc = pastryShop.ShortDesc,
                LongDesc = pastryShop.LongDesc,
                CoverPic = pastryShop.CoverPic,
                ProfilePic = pastryShop.ProfilePic,
                PriceRange_FK = pastryShop.PriceRange_FK,
                Address_FK = pastryShop.Address_FK,
                Categories = pastryShop.Categories
	        };
            toRemoveCategories.ForEach(rc => p.Categories.Remove(p.Categories.FirstOrDefault(c => c.ID == rc.ID)));
            newSelectedCategories.ForEach(sc => p.Categories.Add(sc));
            var pastryShopRC = new PastryShopRestClient();
	        try
	        {
                if (await pastryShopRC.PutAsyncCategories(p.ID, p))
                {
                    await PopupNavigation.PopAllAsync();
                    await DisplayAlert("Succées", "Liste de catégories mise à jours!", "Ok");
                    //editProfileInfo.UpdateParent = true;
                    editProfileInfo.load();
                }
            }
	        catch (HttpRequestException)
	        {
                await PopupNavigation.PopAllAsync();
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
	        }
	    }
    }
}
