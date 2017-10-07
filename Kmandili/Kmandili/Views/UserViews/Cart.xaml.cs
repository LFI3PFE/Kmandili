using Kmandili.Models;
using Kmandili.Models.LocalModels;
using Kmandili.Models.RestClient;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Kmandili.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.UserViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Cart
	{
		public Cart ()
		{
			InitializeComponent ();

            UpdateView();
        }

        private void UpdateView()
        {
            MainLayout.Children.Clear();
            ToolbarItems.Clear();

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
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
#pragma warning restore CS0618 // Type or member is obsolete
            confirmToolbarItem.Clicked += ConfirmToolbarItemClicked;
            clearToolbarItem.Clicked += ClearToolbarItemClicked;
            ToolbarItems.Add(confirmToolbarItem);
            ToolbarItems.Add(clearToolbarItem);
            //PastryList.ItemsSource = App.Cart;
            MainLayout.Children.Add(MakeCartPastryView());

            if (App.Cart.Count == 0)
            {
                MainLayout.IsVisible = false;
                CartTotalStackLayout.IsVisible = false;
                EmptyCartLabel.IsVisible = true;
                
                ToolbarItems.Remove(confirmToolbarItem);
                ToolbarItems.Remove(clearToolbarItem);
            }
            CartTotal.Text = App.Cart.Sum(p => p.Total).ToString(CultureInfo.InvariantCulture);
        }

        private StackLayout MakeCartPastryView()
        {
            StackLayout mainStackLayout = new StackLayout();
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
                Picker deleveryMethodPicker = new Picker
                {
                    ItemsSource = cartPastry.PastryShop.PastryShopDeleveryMethods.ToList(),
                    ClassId = cartPastry.PastryShop.ID.ToString(),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    SelectedIndex = 0
                };
                cartPastry.DeleveryMethod = cartPastry.PastryShop.PastryShopDeleveryMethods.ElementAt(0).DeleveryMethod;
                deleveryMethodPicker.SelectedIndexChanged += DeleveryMethodPicker_SelectedIndexChanged;

                innerDeleveryLayout.Children.Add(deleveryMethodPicker);
                innerDeleveryLayout.Children.Add(new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label() { Text = "Delais: ", TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold},
                        new Label() { Text = (deleveryMethodPicker.SelectedItem as PastryShopDeleveryMethod)?.DeleveryDelay.ToString(), TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center }
                    }
                });
                
                deleveryLayout.Children.Add(new Label() { Text = "Livraison:", TextColor = Color.Black, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold});
                deleveryLayout.Children.Add(innerDeleveryLayout);

                StackLayout paymentLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand, Spacing = 10, Padding = new Thickness(10, 0, 10, 0) };
                Picker paymentPicker = new Picker
                {
                    ItemsSource =
                        cartPastry.PastryShop.PastryShopDeleveryMethods.ElementAt(deleveryMethodPicker.SelectedIndex)
                            .PastryDeleveryPayments.ToList(),
                    ClassId = cartPastry.PastryShop.ID.ToString(),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    SelectedIndex = 0
                };
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
                footerStackLayout.Children.Add(new Label() { Text = cartPastry.Total.ToString(CultureInfo.InvariantCulture), TextColor = Color.Gray, FontSize = 25 });
                footerStackLayout.Children.Add(new Label() { Text = "TND", TextColor = Color.Gray, FontSize = 20 });

                StackLayout cellStack = new StackLayout()
                {
                    Padding = new Thickness(0, 0, 0, 20),
                };
                cellStack.Children.Add(headerStackLayout);
                cellStack.Children.Add(productStackLayout);
                cellStack.Children.Add(footerStackLayout);

                mainStackLayout.Children.Add(cellStack);
            }
            return mainStackLayout;
        }

        private void PaymentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((sender as Picker)?.SelectedItem != null)
            {
                var cartObject = App.Cart
                    .FirstOrDefault(c => c.PastryShop.ID == Int32.Parse((sender as Picker)?.ClassId ?? throw new InvalidOperationException()));
                if (cartObject != null)
                    cartObject.PaymentMethod = (((Picker) sender).SelectedItem as PastryDeleveryPayment)?.Payment;
            }
        }

        private void DeleveryMethodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cartObject = App.Cart.FirstOrDefault(c => c.PastryShop.ID == Int32.Parse(((Picker) sender).ClassId));
            if (cartObject != null)
                cartObject.DeleveryMethod =
                    (((Picker) sender).SelectedItem as PastryShopDeleveryMethod)?.DeleveryMethod;
            var label = ((Label) (((sender as Picker)?.Parent as StackLayout)?.Children[1] as StackLayout)?.Children[1]);
            if (label != null)
                label.Text = (((Picker) sender).SelectedItem as PastryShopDeleveryMethod)?.DeleveryDelay.ToString();
            Picker paymentPicker = ((Picker) (((((sender as Picker)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Children[2] as StackLayout)?.Children[1]);
            if (paymentPicker != null)
            {
                paymentPicker.ItemsSource = App.Cart.FirstOrDefault(c => c.PastryShop.ID == Int32.Parse((sender as Picker)?.ClassId ?? throw new InvalidOperationException()))?.PastryShop.PastryShopDeleveryMethods.ElementAt(((Picker) sender).SelectedIndex).PastryDeleveryPayments.ToList();
                paymentPicker.SelectedIndex = 0;
            }
        }

        private StackLayout MakeProductLayout(CartProduct cartProduct)
        {
            StackLayout mainStackLayout = new StackLayout()
            {
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
            };
            //******   Cell Layout   *******\\
            StackLayout cellLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                HeightRequest = 100,
            };

            //******   Image Layout   *******\\
            StackLayout imageStackLayout = new StackLayout()
            {
                Padding = 5,
                VerticalOptions = LayoutOptions.Center,
            };
            StackLayout imagePlaceHolderStackLayout = new StackLayout()
            {
                BackgroundColor = Color.Gray,
                HeightRequest = 80,
                WidthRequest = 129
            };
            Image productImage = new Image()
            {
                Source = App.ServerUrl + "Uploads/" + cartProduct.Product.Pic,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 80
            };
            imagePlaceHolderStackLayout.Children.Add(productImage);
            imageStackLayout.Children.Add(imagePlaceHolderStackLayout);
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
            removeIconTapGesture.Tapped += RemoveFromCart;
            removeIcon.GestureRecognizers.Add(removeIconTapGesture);
            removeIconStackLayout.Children.Add(removeIcon);

#pragma warning disable CS0618 // Type or member is obsolete
            headerGrid.Children.Add(new Label() { Text = cartProduct.Product.Name, TextColor = Color.Black, FontSize = Device.OnPlatform(25,30,25) }, 0, 0);
#pragma warning restore CS0618 // Type or member is obsolete
            headerGrid.Children.Add(removeIconStackLayout, 1, 0);
            //*************\\

            //******   ProductInfo Layout   *******\\
            StackLayout infoLayout = new StackLayout();
            StackLayout priceLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(10, 0, 0, 0) };
            priceLayout.Children.Add(new Label() { Text = "Prix:", FontSize = 20, TextColor = Color.Black });
            priceLayout.Children.Add(new Label() { Text = cartProduct.Product.Price.ToString(CultureInfo.InvariantCulture), FontSize = 20, TextColor = Color.Black });
            priceLayout.Children.Add(new Label() { Text = "TND", FontSize = 18, TextColor = Color.Gray });
            priceLayout.Children.Add(new Label() { Text = "/", FontSize = 20, TextColor = Color.Black });
            priceLayout.Children.Add(new Label() { Text = cartProduct.Product.SaleUnit.Unit, FontSize = 20, TextColor = Color.Black });
            //*************\\
            //******   Quantity&Total Layout   *******\\
            StackLayout quantityTotalLayout = new StackLayout();
            StackLayout quantityLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 5 };
            quantityLayout.Children.Add(new Label() { Text = "Quantité:", FontSize = 20, TextColor = Color.Black });
            quantityLayout.Children.Add(new Label() { Text = cartProduct.Quantity.ToString(), FontSize = 20, TextColor = Color.Black });

            StackLayout totalLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 5 };
            quantityLayout.Children.Add(new Label() { Text = "Total:", FontSize = 20, TextColor = Color.Black });
            quantityLayout.Children.Add(new Label() { Text = cartProduct.Total.ToString(CultureInfo.InvariantCulture), FontSize = 20, TextColor = Color.Black });
            quantityLayout.Children.Add(new Label() { Text = "TND", FontSize = 18, TextColor = Color.Gray });

            quantityTotalLayout.Children.Add(quantityLayout);
            quantityTotalLayout.Children.Add(totalLayout);
            //*************\\
            infoLayout.Children.Add(priceLayout);
            infoLayout.Children.Add(quantityTotalLayout);

            productInformationStackLayout.Children.Add(headerGrid);
            productInformationStackLayout.Children.Add(infoLayout);

            cellLayout.Children.Add(imageStackLayout);
            cellLayout.Children.Add(productInformationStackLayout);
            //*************\\
            mainStackLayout.Children.Add(new Label() { Text = cartProduct.Product.ID.ToString(), IsVisible = false });
            mainStackLayout.Children.Add(cellLayout);
            //*************\\
            return mainStackLayout;
        }

        private async void ConfirmToolbarItemClicked(object sender, EventArgs e)
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
                RestClient<Order> orderRc = new RestClient<Order>();
                try
                {
                    order = await orderRc.PostAsync(order);
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
                if(order != null)
                {
                    RestClient<OrderProduct> orderProductRc = new RestClient<OrderProduct>();
                    foreach (CartProduct cartProduct in cartPastry.CartProducts)
                    {
                        OrderProduct orderProduct = new OrderProduct()
                        {
                            Product_FK = cartProduct.Product.ID,
                            Order_FK = order.ID,
                            Quantity = cartProduct.Quantity
                        };
                        try
                        {
                            orderProduct = await orderProductRc.PostAsync(orderProduct);
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
                        if (orderProduct == null)
                        {
                            await DisplayAlert("Erreur", "Erreur lors de l'enregistrement de la commande!", "Ok");
                            await PopupNavigation.PopAsync();
                            return;
                        }
                    }
                    EmailRestClient emailRc = new EmailRestClient();
                    try
                    {
                        await emailRc.SendOrderEmail(order.ID);
                    }
                    catch (HttpRequestException)
                    {
                        await PopupNavigation.PopAllAsync();
                        await DisplayAlert("Erreur", "Une erreur s'est produite lors de la communication avec le serveur, veuillez réessayer plus tard.", "Ok");
                    }
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
            UpdateView();
        }

        private void ClearToolbarItemClicked(object sender, EventArgs e)
        {
            App.Cart.Clear();
            UpdateView();
        }

	    private void RemoveFromCart(object sender, EventArgs e)
        {
            Image image = (Image)sender;
            int id = Int32.Parse(((((((image.Parent as StackLayout)?.Parent as Grid)?.Parent as StackLayout)?.Parent as StackLayout)?.Parent as StackLayout)?.Children[0] as Label)?.Text ?? throw new InvalidOperationException());
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
            done:
            InitializeComponent();
            UpdateView();
        }
    }
}
