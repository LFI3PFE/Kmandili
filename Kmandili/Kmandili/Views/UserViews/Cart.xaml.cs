using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Cart : ContentPage
	{
		public Cart ()
		{
			InitializeComponent ();

            updateView();
        }

        private void updateView()
        {
            MainLayout.Children.Clear();
            this.ToolbarItems.Clear();

#pragma warning disable CS0618 // Type or member is obsolete
            ToolbarItem confirmToolbarItem = new ToolbarItem
            {
                Icon = "confirm.png",
                Text = "Confirmer",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            ToolbarItem clearToolbarItem = new ToolbarItem
            {
                Icon = "emptyCart.png",
                Text = "Vider",
                Order =
                    Device.OnPlatform(ToolbarItemOrder.Secondary, ToolbarItemOrder.Secondary, ToolbarItemOrder.Primary),
                Priority = 2
            };
#pragma warning restore CS0618 // Type or member is obsolete
            confirmToolbarItem.Clicked += confirmToolbarItemClicked;
            clearToolbarItem.Clicked += clearToolbarItemClicked;
            this.ToolbarItems.Add(confirmToolbarItem);
            this.ToolbarItems.Add(clearToolbarItem);
            //PastryList.ItemsSource = App.Cart;
            MainLayout.Children.Add(MakeCartPastryView());

            if (App.Cart.Count == 0)
            {
                MainLayout.IsVisible = false;
                CartTotalStackLayout.IsVisible = false;
                EmptyCartLabel.IsVisible = true;
                
                this.ToolbarItems.Remove(confirmToolbarItem);
                this.ToolbarItems.Remove(clearToolbarItem);
            }
            CartTotal.Text = App.Cart.Sum(p => p.Total).ToString();
        }

        private StackLayout MakeCartPastryView()
        {
            StackLayout MainStackLayout = new StackLayout();
            foreach(CartPastry cartPastry in App.Cart)
            {
                StackLayout headerStackLayout = new StackLayout()
                {
                    Padding = new Thickness(10, 0, 0, 0),
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 10
                };

                StackLayout deleveryLayout = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(10, 0, 10, 0) };
                StackLayout innerDeleveryLayout = new StackLayout() {Orientation = StackOrientation.Horizontal, Spacing = 20};
                Picker deleveryMethodPicker = new Picker() { ItemsSource = cartPastry.PastryShop.PastryShopDeleveryMethods.ToList(), ClassId = cartPastry.PastryShop.ID.ToString(), HorizontalOptions = LayoutOptions.FillAndExpand};
                deleveryMethodPicker.SelectedIndex = 0;
                cartPastry.DeleveryMethod = cartPastry.PastryShop.PastryShopDeleveryMethods.ElementAt(0).DeleveryMethod;
                deleveryMethodPicker.SelectedIndexChanged += DeleveryMethodPicker_SelectedIndexChanged;

                innerDeleveryLayout.Children.Add(deleveryMethodPicker);
                innerDeleveryLayout.Children.Add(new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label() { Text = "Delais: ", TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold},
                        new Label() { Text = (deleveryMethodPicker.SelectedItem as PastryShopDeleveryMethod).DeleveryDelay.ToString(), TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center }
                    }
                });
                
                deleveryLayout.Children.Add(new Label() { Text = "Livraison:", TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold});
                deleveryLayout.Children.Add(innerDeleveryLayout);

                StackLayout paymentLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand, Spacing = 10, Padding = new Thickness(10, 0, 10, 0) };
                Picker paymentPicker = new Picker() { ItemsSource = cartPastry.PastryShop.PastryShopDeleveryMethods.ElementAt(deleveryMethodPicker.SelectedIndex).PastryDeleveryPayments.ToList(), ClassId = cartPastry.PastryShop.ID.ToString(), HorizontalOptions = LayoutOptions.FillAndExpand };
                paymentPicker.SelectedIndex = 0;
                cartPastry.PaymentMethod = cartPastry.PastryShop.PastryShopDeleveryMethods.ElementAt(deleveryMethodPicker.SelectedIndex).PastryDeleveryPayments.ElementAt(0).Payment;
                paymentPicker.SelectedIndexChanged += PaymentPicker_SelectedIndexChanged;
                paymentLayout.Children.Add(new Label() { Text = "Payment:", TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold});
                paymentLayout.Children.Add(paymentPicker);

                headerStackLayout.Children.Add(new Label() { Text = cartPastry.PastryShop.Name, TextColor = Color.Black, FontSize = 25 });
                headerStackLayout.Children.Add(deleveryLayout);
                headerStackLayout.Children.Add(paymentLayout);

                StackLayout productStackLayout = new StackLayout();
                foreach(CartProduct cartProduct in cartPastry.CartProducts)
                {
                    productStackLayout.Children.Add(MakeProductLayout(cartProduct));
                }

                StackLayout footerStackLayout = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.End,
                    Padding = new Thickness(0, 0, 10, 0),
                    Orientation = StackOrientation.Horizontal,
                };
                footerStackLayout.Children.Add(new Label() { Text = "Total:", TextColor = Color.Gray, FontSize = 25 });
                footerStackLayout.Children.Add(new Label() { Text = cartPastry.Total.ToString(), TextColor = Color.Gray, FontSize = 25 });
                footerStackLayout.Children.Add(new Label() { Text = "TND", TextColor = Color.Gray, FontSize = 20 });

                StackLayout cellStack = new StackLayout()
                {
                    Padding = new Thickness(0, 0, 0, 20),
                };
                cellStack.Children.Add(headerStackLayout);
                cellStack.Children.Add(productStackLayout);
                cellStack.Children.Add(footerStackLayout);

                MainStackLayout.Children.Add(cellStack);
            }
            return MainStackLayout;
        }

        private void PaymentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((sender as Picker).SelectedItem != null)
            {
                App.Cart.FirstOrDefault(c => c.PastryShop.ID == Int32.Parse((sender as Picker).ClassId)).PaymentMethod = ((sender as Picker).SelectedItem as PastryDeleveryPayment).Payment;
            }
        }

        private void DeleveryMethodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            App.Cart.FirstOrDefault(c => c.PastryShop.ID == Int32.Parse((sender as Picker).ClassId)).DeleveryMethod = ((sender as Picker).SelectedItem as PastryShopDeleveryMethod).DeleveryMethod;
            ((((sender as Picker).Parent as StackLayout).Children[1] as StackLayout).Children[1] as Label).Text =
                ((sender as Picker).SelectedItem as PastryShopDeleveryMethod).DeleveryDelay.ToString();
            Picker paymentPicker = ((((((sender as Picker).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[2] as StackLayout).Children[1] as Picker);
            paymentPicker.ItemsSource = App.Cart.FirstOrDefault(c => c.PastryShop.ID == Int32.Parse((sender as Picker).ClassId)).PastryShop.PastryShopDeleveryMethods.ElementAt((sender as Picker).SelectedIndex).PastryDeleveryPayments.ToList();
            paymentPicker.SelectedIndex = 0;
        }

        private StackLayout MakeProductLayout(CartProduct cartProduct)
        {
            StackLayout MainStackLayout = new StackLayout()
            {
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
            };
            //******   Cell Layout   *******\\
            StackLayout CellLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                HeightRequest = 100,
            };

            //******   Image Layout   *******\\
            StackLayout ImageStackLayout = new StackLayout()
            {
                Padding = 5,
                VerticalOptions = LayoutOptions.Center,
            };
            StackLayout ImagePlaceHolderStackLayout = new StackLayout()
            {
                BackgroundColor = Color.Gray,
                HeightRequest = 80,
                WidthRequest = 129
            };
            Image productImage = new Image()
            {
                Source = cartProduct.Product.Pic,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 80
            };
            ImagePlaceHolderStackLayout.Children.Add(productImage);
            ImageStackLayout.Children.Add(ImagePlaceHolderStackLayout);
            //*************\\

            //******   Product Information Layout   *******\\
            StackLayout productInformationStackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 1,
            };
            //******   ProductName&Header Layout   *******\\
            Grid headerGrid= new Grid() { HorizontalOptions = LayoutOptions.FillAndExpand};
            headerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star});
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

            StackLayout removeIconStackLayout = new StackLayout() { HorizontalOptions = LayoutOptions.End, Padding = new Thickness(0, 5, 10, 0) };
            Image removeIcon= new Image() { Source = "delete.png", HeightRequest = 22, HorizontalOptions= LayoutOptions.End};
            TapGestureRecognizer removeIconTapGesture = new TapGestureRecognizer();
            removeIconTapGesture.Tapped += removeFromCart;
            removeIcon.GestureRecognizers.Add(removeIconTapGesture);
            removeIconStackLayout.Children.Add(removeIcon);

#pragma warning disable CS0618 // Type or member is obsolete
            headerGrid.Children.Add(new Label() { Text = cartProduct.Product.Name, TextColor = Color.Black, FontSize = Device.OnPlatform(25,30,25) }, 0, 0);
#pragma warning restore CS0618 // Type or member is obsolete
            headerGrid.Children.Add(removeIconStackLayout, 1, 0);
            //*************\\

            //******   ProductInfo Layout   *******\\
            StackLayout InfoLayout = new StackLayout();
            StackLayout PriceLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 0, 0, 0) };
            PriceLayout.Children.Add(new Label() { Text = "Prix:", FontSize = 20, TextColor = Color.Black });
            PriceLayout.Children.Add(new Label() { Text = cartProduct.Product.Price.ToString(), FontSize = 20, TextColor = Color.Black });
            PriceLayout.Children.Add(new Label() { Text = "TND", FontSize = 18, TextColor = Color.Gray });
            PriceLayout.Children.Add(new Label() { Text = "/", FontSize = 20, TextColor = Color.Black });
            PriceLayout.Children.Add(new Label() { Text = cartProduct.Product.SaleUnit.Unit, FontSize = 20, TextColor = Color.Black });
            //*************\\
            //******   Quantity&Total Layout   *******\\
            StackLayout QuantityTotalLayout = new StackLayout();
            StackLayout QuantityLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 5 };
            QuantityLayout.Children.Add(new Label() { Text = "Quantité:", FontSize = 20, TextColor = Color.Black });
            QuantityLayout.Children.Add(new Label() { Text = cartProduct.Quantity.ToString(), FontSize = 20, TextColor = Color.Black });

            StackLayout TotalLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 5 };
            QuantityLayout.Children.Add(new Label() { Text = "Total:", FontSize = 20, TextColor = Color.Black });
            QuantityLayout.Children.Add(new Label() { Text = cartProduct.Total.ToString(), FontSize = 20, TextColor = Color.Black });
            QuantityLayout.Children.Add(new Label() { Text = "TND", FontSize = 18, TextColor = Color.Gray });

            QuantityTotalLayout.Children.Add(QuantityLayout);
            QuantityTotalLayout.Children.Add(TotalLayout);
            //*************\\
            InfoLayout.Children.Add(PriceLayout);
            InfoLayout.Children.Add(QuantityTotalLayout);

            productInformationStackLayout.Children.Add(headerGrid);
            productInformationStackLayout.Children.Add(InfoLayout);

            CellLayout.Children.Add(ImageStackLayout);
            CellLayout.Children.Add(productInformationStackLayout);
            //*************\\
            MainStackLayout.Children.Add(new Label() { Text = cartProduct.Product.ID.ToString(), IsVisible = false });
            MainStackLayout.Children.Add(CellLayout);
            //*************\\
            return MainStackLayout;
        }

        private async void confirmToolbarItemClicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            foreach(CartPastry cartPastry in App.Cart)
            {
                Order order = new Order()
                {
                    User_FK = Settings.Id,
                    PastryShop_FK = cartPastry.PastryShop.ID,
                    DeleveryMethod_FK = cartPastry.DeleveryMethod.ID,
                    PaymentMethod_FK = cartPastry.PaymentMethod.ID,
                    Status_FK = 1,
                    SeenUser = false,
                    SeenPastryShop = false,
                };
                RestClient<Order> orderRC = new RestClient<Order>();
                order = await orderRC.PostAsync(order);
                if(order != null)
                {
                    RestClient<OrderProduct> orderProductRC = new RestClient<OrderProduct>();
                    foreach (CartProduct cartProduct in cartPastry.CartProducts)
                    {
                        OrderProduct orderProduct = new OrderProduct()
                        {
                            Product_FK = cartProduct.Product.ID,
                            Order_FK = order.ID,
                            Quantity = cartProduct.Quantity
                        };
                        orderProduct = await orderProductRC.PostAsync(orderProduct);
                        if (orderProduct == null)
                        {
                            await DisplayAlert("Erreur", "Erreur lors de l'enregistrement de la commande!", "Ok");
                            await PopupNavigation.PopAsync();
                            return;
                        }
                    }
                    EmailRestClient emailRC = new EmailRestClient();
                    await emailRC.SendOrderEmail(order.ID);
                }
                else
                {
                    await DisplayAlert("Erreur", "Erreur lors de l'enregistrement de la commande!", "Ok");
                    await PopupNavigation.PopAsync();
                    return;
                }
            }
            await DisplayAlert("Succès", "Votre Commande a été passée avec succès.\nUn email vous a été envoyé avec plus d'informations.", "Ok");
            await PopupNavigation.PopAsync();
            App.Cart.Clear();
            updateView();
        }

        private void clearToolbarItemClicked(object sender, EventArgs e)
        {
            App.Cart.Clear();
            updateView();
        }

        private void SelectedNot(Object sender, ItemTappedEventArgs e)
        {
            ListView listView = sender as ListView;
            listView.SelectedItem = null;
        }

        private void removeFromCart(object sender, EventArgs e)
        {
            Image image = (Image)sender;
            int id = Int32.Parse(((((((image.Parent as StackLayout).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Parent as StackLayout).Children[0] as Label).Text);
            foreach(CartPastry cartPastry in App.Cart)
            {
                foreach(CartProduct cartProduct in cartPastry.CartProducts)
                {
                    if(cartProduct.Product.ID == id)
                    {
                        if (cartProduct.Quantity == 1)
                        {
                            if(cartPastry.CartProducts.Count == 1)
                            {
                                App.Cart.Remove(cartPastry);
                                goto done;
                            }
                            cartPastry.CartProducts.Remove(cartProduct);
                            goto done;
                        }else
                        {
                            cartProduct.Quantity--;
                            cartProduct.updateTotal();
                            goto done;
                        }
                    }
                }
            }
            done:;
            InitializeComponent();
            updateView();
        }
    }
}
