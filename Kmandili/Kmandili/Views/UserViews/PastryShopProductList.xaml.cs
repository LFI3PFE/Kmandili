using Kmandili.Models;
using Kmandili.Models.LocalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopProductList : ContentPage
	{
        private PastryShop pastryShop;
		public PastryShopProductList (PastryShop pastryShop)
		{
			InitializeComponent ();
            this.pastryShop = pastryShop;
            load();
		}

        private void load()
        {
            List.ItemsSource = pastryShop.Products;
        }

        public void SelectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        public async void addToCart(object sender, EventArgs e)
        {
            int id = Int32.Parse((((((((sender as Image).Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            Product product = pastryShop.Products.FirstOrDefault(p => p.ID == id);
            var index = App.Cart.FindIndex(p => p.PastryShop.ID == product.PastryShop.ID);
            if (App.Cart.Count == 0 || index < 0)
            {
                CartPastry cartPastry = new CartPastry()
                {
                    PastryShop = product.PastryShop,
                };
                CartProduct cartProduct = new CartProduct()
                {
                    Product = product,
                    Quantity = 1
                };
                cartProduct.updateTotal();

                cartPastry.CartProducts.Add(cartProduct);
                App.Cart.Add(cartPastry);
            }
            else
            {
                CartProduct cartP = App.Cart.ElementAt(index).CartProducts.FirstOrDefault(p => p.Product.ID == product.ID);
                if (cartP == null)
                {
                    CartProduct cartProduct = new CartProduct()
                    {
                        Product = product,
                        Quantity = 1
                    };
                    cartProduct.updateTotal();
                    App.Cart.ElementAt(index).CartProducts.Add(cartProduct);
                }
                else
                {
                    cartP.Quantity++;
                    cartP.updateTotal();
                }
            }
            await DisplayAlert("Succée", "Produit Ajouté au pannier", "OK");
        }
    }
}
