using System;
using System.Net.Http;
using Kmandili.Models;
using Kmandili.Models.RestClient;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.EditProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDeleveryMethods
	{
	    private readonly EditProfileInfo _editProfileInfo;
        private PastryShop _pastryShop;
	    private bool _reloadParent;
	    private readonly int _id;

		public EditDeleveryMethods (EditProfileInfo editProfileInfo, int id)
		{
		    _editProfileInfo = editProfileInfo;
		    _id = id;
            InitializeComponent ();
            var addToolbarItem = new ToolbarItem()
            {
                Text = "Ajouter",
                Order = ToolbarItemOrder.Primary,
                Icon = "plus.png"
            };
            addToolbarItem.Clicked += AddToolbarItem_Clicked;
		    ToolbarItems.Add(addToolbarItem);
            Load(_reloadParent);
		}

        private async void AddToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new PEditAddDeleveryMethodForm(this, _pastryShop));
        }

        public async void Load(bool reloadParentval)
        {
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            _reloadParent = reloadParentval;
	        PastryShopRestClient pastryShopRc = new PastryShopRestClient();
            try
            {
                _pastryShop = await pastryShopRc.GetAsyncById(_id);
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
            if (_pastryShop == null)
            {
                await PopupNavigation.PopAllAsync();
                return;
            }
            ContentLayout.Children.Clear();
	        foreach (var pastryShopDeleveryMethod in _pastryShop.PastryShopDeleveryMethods)
	        {
	            ContentLayout.Children.Add(MakeDeleveryMethodLayout(pastryShopDeleveryMethod));
	        }
            await PopupNavigation.PopAllAsync(); 
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
            await PopupNavigation.PushAsync(new LoadingPopupPage());
            if (_pastryShop.PastryShopDeleveryMethods.Count == 1)
            {
                await PopupNavigation.PopAsync();
                await DisplayAlert("Erreur", "Il faut avoir au moins une methode de livraison!", "Ok");
                return;
            }
            int id = Int32.Parse((sender as StackLayout)?.ClassId);
            RestClient<PastryShopDeleveryMethod> pastryShopDeleverMethodRc = new RestClient<PastryShopDeleveryMethod>();
            try
            {
                if (!(await pastryShopDeleverMethodRc.DeleteAsync(id)))
                {
                    await PopupNavigation.PopAsync();
                    return;
                }
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
            await PopupNavigation.PopAsync();
            Load(true);
        }

	    protected override void OnDisappearing()
	    {
	        if (_reloadParent)
	        {
                _editProfileInfo.Load();
            }
	    }
	}
}
