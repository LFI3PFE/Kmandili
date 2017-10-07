using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCategories
	{
	    private List<Category> _categories; 
        private readonly List<Category> _newSelectedCategories = new List<Category>();
        private readonly List<Category> _toRemoveCategories = new List<Category>();
        private readonly List<Category> _allSelectedCategories = new List<Category>();

	    private readonly EditProfileInfo _editProfileInfo;

	    private readonly PastryShop _pastryShop;

		public EditCategories (EditProfileInfo editProfileInfo, PastryShop pastryShop)
		{
            BackgroundColor = Color.FromHex("#CC000000");
            _pastryShop = pastryShop;
		    _editProfileInfo = editProfileInfo;
			InitializeComponent ();
		    Load();
		}

	    private async void Load()
	    {
            _pastryShop.Categories.ToList().ForEach(c => _allSelectedCategories.Add(c));
	        var categoryRc = new RestClient<Category>();
	        try
	        {
                _categories = await categoryRc.GetAsync();
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
	        if (_categories == null) return;
            CategoriesLayout.Children.Clear();
            _categories.ForEach(c => CategoriesLayout.Children.Add(MakeCategoryLayout(c)));
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
                IsToggled = _pastryShop.Categories.Any(c => c.ID == category.ID)
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
            var senderSwitch = ((Switch) sender);
            if (senderSwitch == null) return;
            if (senderSwitch.IsToggled)
            {
                _allSelectedCategories.Add(_categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                if (_pastryShop.Categories.Any(c => c.ID == Int32.Parse(senderSwitch.ClassId)))
                {
                    _toRemoveCategories.Remove(_toRemoveCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
                else
                {
                    _newSelectedCategories.Add(_categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
            }
            else
            {
                _allSelectedCategories.Remove(_allSelectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                if (_pastryShop.Categories.Any(c => c.ID == Int32.Parse(senderSwitch.ClassId)))
                {
                    _toRemoveCategories.Add(_pastryShop.Categories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
                else
                {
                    _newSelectedCategories.Remove(_newSelectedCategories.FirstOrDefault(c => c.ID == Int32.Parse(senderSwitch.ClassId)));
                }
            }
        }

	    private async void Dismiss(object sender, EventArgs e)
	    {
	        await PopupNavigation.PopAsync();
	    }

	    private async void Apply(object sender, EventArgs e)
	    {
	        if (!_newSelectedCategories.Any() && !_toRemoveCategories.Any())
	        {
	            await PopupNavigation.PopAsync();
                return;
	        }
	        if (!_allSelectedCategories.Any())
	        {
	            await DisplayAlert("Erreur", "Au moins une catégorie doit être selectionné!", "Ok");
                return;
	        }
	        await PopupNavigation.PushAsync(new LoadingPopupPage());
	        foreach (var category in _pastryShop.Categories)
	        {
                category.PastryShops.Clear();
                category.Products.Clear();
            }
	        foreach (var newSelectedCategory in _newSelectedCategories)
	        {
	            newSelectedCategory.PastryShops.Clear();
                newSelectedCategory.Products.Clear();
            }
            PastryShop p = new PastryShop()
	        {
                ID = _pastryShop.ID,
	            Name = _pastryShop.Name,
                Email = _pastryShop.Email,
                Password = _pastryShop.Password,
                ShortDesc = _pastryShop.ShortDesc,
                LongDesc = _pastryShop.LongDesc,
                CoverPic = _pastryShop.CoverPic,
                ProfilePic = _pastryShop.ProfilePic,
                PriceRange_FK = _pastryShop.PriceRange_FK,
                Address_FK = _pastryShop.Address_FK,
                Categories = _pastryShop.Categories
	        };
            _toRemoveCategories.ForEach(rc => p.Categories.Remove(p.Categories.FirstOrDefault(c => c.ID == rc.ID)));
            _newSelectedCategories.ForEach(sc => p.Categories.Add(sc));
            var pastryShopRc = new PastryShopRestClient();
	        try
	        {
                if (await pastryShopRc.PutAsyncCategories(p.ID, p))
                {
                    await PopupNavigation.PopAllAsync();
                    await DisplayAlert("Succées", "Liste de catégories mise à jours!", "Ok");
                    _editProfileInfo.Load();
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
