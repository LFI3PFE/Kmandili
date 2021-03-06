﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryMethods : ContentPage
	{
	    private PastryShopMasterDetailPage pastryShopMasterDetailPage;
	    private PastryShop pastryShop;
	    private ToolbarItem addToolbarItem;
	    private bool reloadParent = false;

		public EditDeleveryMethods (PastryShopMasterDetailPage pastryShopMasterDetailPage)
		{
		    this.pastryShopMasterDetailPage = pastryShopMasterDetailPage;
			InitializeComponent ();
            addToolbarItem = new ToolbarItem()
            {
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Icon = "plus.png"
            };
            addToolbarItem.Clicked += AddToolbarItem_Clicked;
		    ToolbarItems.Add(addToolbarItem);
            Load(reloadParent);
		}

        private async void AddToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new AddDeleveryMethodForm(this, pastryShop));
        }

        public async void Load(bool reloadParentval)
        {
            this.reloadParent = reloadParentval;
	        PastryShopRestClient pastryShopRC = new PastryShopRestClient();
	        pastryShop = await pastryShopRC.GetAsyncById(App.Connected.Id);
            if (pastryShop == null) return;
            ContentLayout.Children.Clear();
	        foreach (var pastryShopDeleveryMethod in pastryShop.PastryShopDeleveryMethods)
	        {
	            ContentLayout.Children.Add(MakeDeleveryMethodLayout(pastryShopDeleveryMethod));
	        }
	    }

	    private StackLayout MakeDeleveryMethodLayout(PastryShopDeleveryMethod pastryShopDeleveryMethod)
	    {
            StackLayout mainLayout = new StackLayout() {BackgroundColor = Color.White};
	        StackLayout innerLayout = new StackLayout() {Padding = new Thickness(20,20,0,20)};
            StackLayout deleveryAndDelayLayout = new StackLayout();
            Grid headerGrid = new Grid();
            headerGrid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Star});
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(5,GridUnitType.Star)});
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            StackLayout deleveryLayout = new StackLayout() {Orientation = StackOrientation.Horizontal, Spacing = 20};
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
            StackLayout removeIconLayout = new StackLayout()
            {
                ClassId = pastryShopDeleveryMethod.ID.ToString()
            };
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
            StackLayout delayLayout = new StackLayout() {Orientation = StackOrientation.Horizontal, Spacing = 20};
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
	            StackLayout paymentLayout = new StackLayout() {Padding = new Thickness(30,0,0,0)};
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

        private async void RemoveGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (pastryShop.PastryShopDeleveryMethods.Count == 1)
            {
                await DisplayAlert("Erreur", "Il faut avoir au moins une methode de livraison!", "Ok");
                return;
            }
            int ID = Int32.Parse((sender as StackLayout).ClassId);
            RestClient<PastryShopDeleveryMethod> pastryShopDeleverMethodRC = new RestClient<PastryShopDeleveryMethod>();
            if(!(await pastryShopDeleverMethodRC.DeleteAsync(ID))) return;
            Load(true);
        }

	    protected override void OnDisappearing()
	    {
	        if (reloadParent)
	        {
                pastryShopMasterDetailPage.ReloadPastryShop();
            }
	    }
	}
}
