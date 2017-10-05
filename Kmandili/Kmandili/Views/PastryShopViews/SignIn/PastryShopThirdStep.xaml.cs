using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopThirdStep : ContentPage
	{
        private List<DeleveryMethod> deleveryMethods;
        private List<Category> categoryList;
        private List<DeleveryDelay> deleveryDelays;
        private PastryShop pastry;
        

        public PastryShopThirdStep(PastryShop pastry)
        {
            InitializeComponent();
            CategoriesListView.SeparatorVisibility = SeparatorVisibility.None;
            this.pastry = pastry;
            Load();
        }
        private async void RemoveGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            int ID = Int32.Parse(((sender as StackLayout).Children[0] as Label).Text);
            pastry.PastryShopDeleveryMethods.Remove(pastry.PastryShopDeleveryMethods.FirstOrDefault(d => d.ID == ID));
            await PopupNavigation.PopAsync();
            RefreshDeleveryMethods();
        }


        private StackLayout MakeDeleveryMethodLayout(PastryShopDeleveryMethod pastryShopDeleveryMethod)
        {
            StackLayout mainLayout = new StackLayout() { BackgroundColor = Color.White };
            StackLayout innerLayout = new StackLayout() { Padding = new Thickness(20, 20, 0, 20) };
            StackLayout deleveryAndDelayLayout = new StackLayout();
            Grid headerGrid = new Grid();
            headerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            StackLayout deleveryLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20 };
            deleveryLayout.Children.Add(new Label()
            {
                Text = "Livraison",
                TextColor = Color.Black,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            });
            deleveryLayout.Children.Add(new Label()
            {
                Text = pastryShopDeleveryMethod.DeleveryMethod.DeleveryType,
                TextColor = Color.Black,
                FontSize = 20
            });
            TapGestureRecognizer removeGestureRecognizer = new TapGestureRecognizer();
            removeGestureRecognizer.Tapped += RemoveGestureRecognizer_Tapped;
            StackLayout removeIconLayout = new StackLayout();
            removeIconLayout.Children.Add(new Label() { Text = pastryShopDeleveryMethod.ID.ToString(), IsVisible = false });
            removeIconLayout.GestureRecognizers.Add(removeGestureRecognizer);
            removeIconLayout.Children.Add(new Image()
            {
                Source = "delete.png",
                WidthRequest = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            });
            headerGrid.Children.Add(deleveryLayout, 0, 0);
            headerGrid.Children.Add(removeIconLayout, 1, 0);
            deleveryAndDelayLayout.Children.Add(headerGrid);
            StackLayout delayLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 20 };
            delayLayout.Children.Add(new Label()
            {
                Text = "Délais:",
                TextColor = Color.Black,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            });
            delayLayout.Children.Add(new Label()
            {
                Text = (pastryShopDeleveryMethod.DeleveryDelay.MinDelay == 0 && pastryShopDeleveryMethod.DeleveryDelay.MaxDelay == 0) ?
                            "Instantané" :
                            pastryShopDeleveryMethod.DeleveryDelay.MaxDelay + "-" + pastryShopDeleveryMethod.DeleveryDelay.MinDelay + " jours",
                TextColor = Color.Black,
                FontSize = 18,
            });
            deleveryAndDelayLayout.Children.Add(delayLayout);
            innerLayout.Children.Add(deleveryAndDelayLayout);
            StackLayout paymentsLayout = new StackLayout();
            paymentsLayout.Children.Add(new Label()
            {
                Text = "Payments:",
                TextColor = Color.Black,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
            });
            foreach (var pastryDeleveryPayment in pastryShopDeleveryMethod.PastryDeleveryPayments)
            {
                StackLayout paymentLayout = new StackLayout() { Padding = new Thickness(30, 0, 0, 0) };
                paymentLayout.Children.Add(new Label()
                {
                    Text = pastryDeleveryPayment.Payment.PaymentMethod,
                    TextColor = Color.Black,
                    FontSize = 18,
                });
                paymentsLayout.Children.Add(paymentLayout);
            }
            innerLayout.Children.Add(paymentsLayout);
            mainLayout.Children.Add(innerLayout);
            return mainLayout;
        }

        private async void AddDeleveryMethoTapped(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new PSignAddDeleveryMethodForm(this, pastry));
        }

	    public void RefreshDeleveryMethods()
	    {
            ContentLayout.Children.Clear();
            if (pastry.PastryShopDeleveryMethods.Count != 0)
            {
                ContentLayout.IsVisible = true;
                NoDeleveryMethoLayout.IsVisible = false;
                int i = 1;
                foreach (var pastryShopDeleveryMethod in pastry.PastryShopDeleveryMethods)
                {
                    pastryShopDeleveryMethod.ID = i;
                    ContentLayout.Children.Add(MakeDeleveryMethodLayout(pastryShopDeleveryMethod));
                    i++;
                }
            }
            else
            {
                ContentLayout.IsVisible = false;
                NoDeleveryMethoLayout.IsVisible = true;
            }
        }

        private async void Load()
        {
            CategoriesContentLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;
            Loading.IsRunning = true;
            RestClient<Category> categoryRC = new RestClient<Category>();
            try
            {
                categoryList = await categoryRC.GetAsync();
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
            if (categoryList == null) return;
            CategoriesListView.HeightRequest = categoryList.Count * 30;
            CategoriesListView.ItemsSource = categoryList;
            Loading.IsRunning = false;
            LoadingLayout.IsVisible = false;
            CategoriesContentLayout.IsVisible = true;
            RefreshDeleveryMethods();
        }

        private void selectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

	    private async void NextBt_Clicked(object sender, EventArgs e)
	    {
	        if (pastry.PastryShopDeleveryMethods.Count == 0)
	        {
                await DisplayAlert("Erreur", "Au moins une methode de livraison avec une methode de payment doit être choisie!", "Ok!");
            }
            else if (pastry.Categories.Count == 0)
	        {
                await DisplayAlert("Erreur", "Au moins une catégorie doit être séléctionnée!", "Ok!");
            }
            else
            {
                await Navigation.PushAsync(new PastryShopUploadPhotos(pastry));
            }
	    }

        private void CategorySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if ((sender as Switch).IsToggled)
            {
                pastry.Categories.Add(categoryList.FirstOrDefault(c => c.ID == Int32.Parse((sender as Switch).ClassId)));
            }
            else
            {
                pastry.Categories.Remove(pastry.Categories.FirstOrDefault(c => c.ID == Int32.Parse((sender as Switch).ClassId)));
            }
        }
    }
}
