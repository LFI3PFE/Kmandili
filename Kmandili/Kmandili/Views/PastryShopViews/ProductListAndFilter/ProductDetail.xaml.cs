using System;
using System.Globalization;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.ProductListAndFilter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetail
	{
	    private DateTime _min;
	    private DateTime _max;
	    private int _year, _semester;
	    private Product _product;

	    private bool _updateParent;
	    private readonly PsProductList _productList;

        public ProductDetail (Product product, PsProductList productList)
		{
		    InitializeComponent ();
            _product = product;
            _productList = productList;
            var refreshToolbarItem = new ToolbarItem()
            {
                Text = "Rafraîchir",
                Order = ToolbarItemOrder.Primary,
                Icon = "refresh.png"
            };
            var editToolbarItem = new ToolbarItem()
            {
                Text = "Modifier",
                Order = ToolbarItemOrder.Primary,
                Icon = "edit.png"
            };
            refreshToolbarItem.Clicked += RefreshToolbarItem_Clicked;
            editToolbarItem.Clicked += EditToolbarItem_Clicked;
            ToolbarItems.Add(refreshToolbarItem);
            ToolbarItems.Add(editToolbarItem);
            _min = product.PastryShop.JoinDate;
            _max = DateTime.Now;
            _year = _max.Year;
            _semester = GetSemester(_max.Month);
            Load(false);
		}

	    private async void EditToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductEditForm(_product, this));
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
                var productRc = new RestClient<Product>();
                try
                {
                    _product = await productRc.GetAsyncById(_product.ID);
                }
                catch (HttpRequestException)
                {
                    await PopupNavigation.PopAllAsync();
                    await
                        DisplayAlert("Erreur",
                            "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.",
                            "Ok");
                    await Navigation.PopAsync();
                    return;
                }
                if (_product == null)
                {
                    LoadingLayout.IsVisible = false;
                    Loading.IsRunning = false;
                    BodyLayout.IsVisible = true;
                    await DisplayAlert("Erreur", "Erreur lors de la récupération des données.", "Ok");
                    return;
                }
            }
            Name.Text = _product.Name;
            Desc.Text = _product.Description;
            Price.Text = _product.Price.ToString(CultureInfo.InvariantCulture);
            SaleUnit.Text = _product.SaleUnit.Unit;
            Category.Text = _product.Category.CategoryName;
            var chartRc = new ChartsRestClient();
            try
            {
                var htmlWebView = new HtmlWebViewSource()
                {
                    Html = await chartRc.GetChartView(App.ServerUrl + "api/GetProductChartView/" + _product.ID + "/" + _year + "/" + _semester)
                };
                ChartWebView.Source = htmlWebView;
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                return;
            }
            if (_max.Year == _year && GetSemester(_max.Month) == _semester)
            {
                SuivantLabel.TextColor = Color.LightSkyBlue;
            }
            else
            {
                SuivantLabel.TextColor = Color.DodgerBlue;
            }

            if (_min.Year == _year && GetSemester(_min.Month) == _semester)
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
            if (_min.Year == _year && GetSemester(_min.Month) == _semester) return;
            if (_semester == 2)
                _semester--;
            else
            {
                _semester = 2;
                _year--;
            }
            Load(false);
        }

	    private void SuivantTapped(object sender, EventArgs e)
	    {
            if (_max.Year == _year && GetSemester(_max.Month) == _semester) return;
            if (_semester == 1)
                _semester++;
            else
            {
                _semester = 1;
                _year++;
            }
            Load(false);
        }

        private int GetSemester(int month)
        {
            return month <= 6 ? 1 : 2;
        }

	    public void Reload()
	    {
	        Load(true);
	        _updateParent = true;
	    }

	    protected override void OnDisappearing()
	    {
	        if (_updateParent)
	        {
	            _productList.Reload();
	        }
	    }
	}
}
