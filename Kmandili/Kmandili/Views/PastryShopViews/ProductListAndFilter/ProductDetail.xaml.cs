using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetail : ContentPage
	{
	    private ToolbarItem refreshToolbarItem, editToolbarItem;
	    private DateTime min;
	    private DateTime max;
	    private int year, semester;
	    private Product product;

	    private bool updateParent = false;
	    private PSProductList productList;

        public ProductDetail (Product product, PSProductList productList)
		{
			InitializeComponent ();
            this.product = product;
            this.productList = productList;
            refreshToolbarItem = new ToolbarItem()
            {
                Text = "Rafraîchir",
                Order = ToolbarItemOrder.Primary,
                Icon = "refresh.png"
            };
            editToolbarItem = new ToolbarItem()
            {
                Text = "Modifier",
                Order = ToolbarItemOrder.Primary,
                Icon = "edit.png"
            };
            refreshToolbarItem.Clicked += RefreshToolbarItem_Clicked;
            editToolbarItem.Clicked += EditToolbarItem_Clicked;
            ToolbarItems.Add(refreshToolbarItem);
            ToolbarItems.Add(editToolbarItem);
            min = product.PastryShop.JoinDate;
            max = DateTime.Now;
            year = max.Year;
            semester = getSemester(max.Month);
            Load(false);
		}

	    private async void EditToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductEditForm(product, this));
        }

        private void RefreshToolbarItem_Clicked(object sender, EventArgs e)
	    {
	        Load(true);
	    }

        private async void Load(bool reload)
        {
            BodyLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            if (reload)
            {
                var productRC = new RestClient<Product>();
                product = await productRC.GetAsyncById(product.ID);
                if (product == null)
                {
                    LoadingLayout.IsVisible = false;
                    Loading.IsRunning = false;
                    BodyLayout.IsVisible = true;
                    await DisplayAlert("Erreur", "Erreur lors de la récupération des données.", "Ok");
                    return;
                }
            }
            Name.Text = product.Name;
            Desc.Text = product.Description;
            Price.Text = product.Price.ToString();
            SaleUnit.Text = product.SaleUnit.Unit;
            Category.Text = product.Category.CategoryName;
            var chartRC = new ChartsRestClient();
            var htmlWebView = new HtmlWebViewSource()
            {
                Html = await chartRC.GetChartView(App.ServerURL + "api/GetProductChartView/" + product.ID + "/" + year + "/" + semester)
            };
            ChartWebView.Source = htmlWebView;
            if (max.Year == year && getSemester(max.Month) == semester)
            {
                SuivantLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                SuivantLabel.TextColor = Color.DodgerBlue;
            }

            if (min.Year == year && getSemester(min.Month) == semester)
            {
                PrecedentLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                PrecedentLabel.TextColor = Color.DodgerBlue;
            }
            LoadingLayout.IsVisible = false;
            Loading.IsRunning = false;
            BodyLayout.IsVisible = true;
        }

	    private void PrecedentTapped(object sender, EventArgs e)
	    {
            if (min.Year == year && getSemester(min.Month) == semester) return;
            if (semester == 2)
                semester--;
            else
            {
                semester = 2;
                year--;
            }
            Load(false);
        }

	    private void SuivantTapped(object sender, EventArgs e)
	    {
            if (max.Year == year && getSemester(max.Month) == semester) return;
            if (semester == 1)
                semester++;
            else
            {
                semester = 1;
                year++;
            }
            Load(false);
        }

        private int getSemester(int month)
        {
            return month <= 6 ? 1 : 2;
        }

	    public void Reload()
	    {
	        Load(true);
	        updateParent = true;
	    }

	    protected override void OnDisappearing()
	    {
	        if (updateParent)
	        {
	            productList.Reload();
	        }
	    }
	}
}
