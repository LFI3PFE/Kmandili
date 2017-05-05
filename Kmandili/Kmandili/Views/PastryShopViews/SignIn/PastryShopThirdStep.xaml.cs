using Kmandili.Models;
using Kmandili.Models.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.PastryShopViews.SignIn
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PastryShopThirdStep : ContentPage
	{
        private class deleveryMethodLocal : INotifyPropertyChanged
        {
            public deleveryMethodLocal()
            {
                selectedPayments = new List<Payment>();
            }

            private int index;
            public int Index { get { return index; } set { index = value; OnPropertyChanged("Index"); } }
            public List<DeleveryMethod> DeleveryMethods { get; set; }
            public List<DeleveryDelay> DeleveryDelays { get; set; }
            public DeleveryMethod selectedDeleveryMethod { get; set; }
            public DeleveryDelay selectedDeleveryDelay { get; set; }
            public List<Payment> selectedPayments { get; set; }

            private bool isLast;
            public bool IsLast { get { return isLast; } set { isLast = value; OnPropertyChanged("IsLast"); } }
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<deleveryMethodLocal> localDeleveryMethods = new ObservableCollection<deleveryMethodLocal>();
        private List<DeleveryMethod> deleveryMethods;
        private List<Category> categoryList;

        private List<int> categorySwitchs = new List<int>();
        private List<DeleveryDelay> deleveryDelays;
        private List<Grid> paymentStacks = new List<Grid>();
        private PastryShop pastry;

        public PastryShopThirdStep(PastryShop pastry)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            CategoriesListView.SeparatorVisibility = SeparatorVisibility.None;
            DeleveryMethodsListView.SeparatorVisibility = SeparatorVisibility.None;
            this.pastry = pastry;
            localDeleveryMethods.CollectionChanged += LocalDeleveryMethods_CollectionChanged;
            load();
        }

        private void LocalDeleveryMethods_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (deleveryMethodLocal local in localDeleveryMethods)
            {
                local.Index = localDeleveryMethods.IndexOf(local);
            }
            if (localDeleveryMethods.Count > 1)
            {
                localDeleveryMethods.ElementAt(localDeleveryMethods.Count - 2).IsLast = false;
            }
            localDeleveryMethods.ElementAt(localDeleveryMethods.Count - 1).IsLast = true;
            DeleveryMethodsListView.HeightRequest = (localDeleveryMethods.Count * 70) + (paymentStacks.Count * 40);
        }

        private async void load()
        {
            RestClient<DeleveryDelay> deleveryDelayRC = new RestClient<DeleveryDelay>();
            deleveryDelays = await deleveryDelayRC.GetAsync();

            RestClient<DeleveryMethod> deleveryMethodRC = new RestClient<DeleveryMethod>();
            deleveryMethods = await deleveryMethodRC.GetAsync();

            RestClient<Category> categoryRC = new RestClient<Category>();
            categoryList = await categoryRC.GetAsync();
            CategoriesListView.HeightRequest = categoryList.Count * 30;
            CategoriesListView.ItemsSource = categoryList;


            deleveryMethodLocal local = new deleveryMethodLocal()
            {
                Index = localDeleveryMethods.Count,
                DeleveryMethods = deleveryMethods,
                DeleveryDelays = deleveryDelays,
                IsLast = true,
                selectedDeleveryMethod = deleveryMethods.First(),
                selectedDeleveryDelay = deleveryDelays.First()
            };
            localDeleveryMethods.Add(local);
            DeleveryMethodsListView.ItemsSource = localDeleveryMethods;
        }

        private void selectedNot(object sender, EventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private void DeleverMethodPicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer" && (sender as Picker).StyleClass.Contains("first"))
            {
                (sender as Picker).SelectedIndex = 0;
                (sender as Picker).StyleClass.Clear();
                (sender as Picker).StyleClass.Add("notfirst");
            }
        }

        private void DeleverMethodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            Picker deleveryPicker = (sender as Picker);
            StackLayout stack1 = (deleveryPicker.Parent as StackLayout);
            StackLayout stack2 = (stack1.Parent as StackLayout);
            StackLayout PaymentStack = (stack2.Children[2] as StackLayout);
            IList<View> s = PaymentStack.Children; //Grids
            if (s.Count != 0)
            {
                s.Clear();
                localDeleveryMethods.FirstOrDefault(l => l.Index == Int32.Parse(deleveryPicker.ClassId)).selectedPayments.Clear();
                localDeleveryMethods.FirstOrDefault(l => l.Index == Int32.Parse(deleveryPicker.ClassId)).selectedDeleveryMethod = deleveryMethods[deleveryPicker.SelectedIndex];
            }
            if ((sender as Picker).SelectedIndex >= 0)
            {
                foreach (Payment payment in deleveryMethods[deleveryPicker.SelectedIndex].Payments)
                {
                    Grid paymentGrid = makePaymentGrid(payment);
                    PaymentStack.Children.Add(paymentGrid);
                    paymentStacks.Add(paymentGrid);
                }
            }
            PaymentStack.HeightRequest = PaymentStack.Children.Count * 40;
            ((deleveryPicker.Parent as StackLayout).Parent as StackLayout).HeightRequest = 70 + (PaymentStack.Children.Count * 40);
            DeleveryMethodsListView.HeightRequest = (localDeleveryMethods.Count * 70) + (paymentStacks.Count * 40);
        }

        private Grid makePaymentGrid(Payment payment)
        {
            Grid grid = new Grid()
            {
                Padding = new Thickness(0, 0, 20, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            Label label = new Label()
            {
                Text = payment.PaymentMethod,
                TextColor = Color.Black,
                FontSize = 18,
            };

            Switch paymentSwitch = new Switch()
            {
                ClassId = payment.ID.ToString(),
            };

            paymentSwitch.Toggled += PaymentMethodToggeled;

            grid.Children.Add(label, 0, 0);
            grid.Children.Add(paymentSwitch, 1, 0);

            return grid;
        }

        private void Add_Tapped(object sender, EventArgs e)
        {
            localDeleveryMethods.Add(new deleveryMethodLocal()
            {
                Index = localDeleveryMethods.Count,
                DeleveryMethods = deleveryMethods,
                DeleveryDelays = deleveryDelays,
                selectedDeleveryMethod = deleveryMethods.First(),
                selectedDeleveryDelay = deleveryDelays.First(),
            });
        }

        private void Remove_Tapped(object sender, EventArgs e)
        {
            int x = Int32.Parse((sender as Image).ClassId);
            StackLayout s = (((sender as Image).Parent as StackLayout).Parent as StackLayout).Children[2] as StackLayout;
            foreach (Grid g in s.Children)
            {
                paymentStacks.Remove(g);
            }
            localDeleveryMethods.RemoveAt(x);
        }

        private async void NextBt_Clicked(object sender, EventArgs e)
        {
            List<PastryShopDeleveryMethod> PSdeleveryMethods = new List<PastryShopDeleveryMethod>();
            foreach (deleveryMethodLocal delevery in localDeleveryMethods)
            {
                if (delevery.selectedPayments.Count != 0)
                {
                    PastryShopDeleveryMethod psdm = PSdeleveryMethods.FirstOrDefault(p => p.DeleveryMethod_FK == delevery.selectedDeleveryMethod.ID);
                    if (psdm == null)
                    {
                        psdm = new PastryShopDeleveryMethod()
                        {
                            DeleveryDelay_FK = delevery.selectedDeleveryDelay.ID,
                            DeleveryMethod_FK = delevery.selectedDeleveryMethod.ID,
                        };
                        PSdeleveryMethods.Add(psdm);
                    }
                    foreach (Payment payment in delevery.selectedPayments)
                    {
                        PastryDeleveryPayment newPDP = new PastryDeleveryPayment
                        {
                            Payment_FK = payment.ID,
                            PastryShopDeleveryMethod_FK = psdm.ID
                        };
                        if (psdm.PastryDeleveryPayments.FirstOrDefault(x => x.Payment_FK == newPDP.Payment_FK) == null)
                        {
                            psdm.PastryDeleveryPayments.Add(newPDP);
                        }
                    }
                }
            }
            if (PSdeleveryMethods.Count == 0)
            {
                await DisplayAlert("Erreur", "Au moins une methode de livraison avec une methode de payment doit être fournit!", "Ok!");
                return;
            }
            if (categorySwitchs.Count == 0)
            {
                await DisplayAlert("Erreur", "Au moins une catégorie doit être séléctionné!", "Ok!");
                return;
            }
            else
            {
                foreach (int i in categorySwitchs)
                {
                    pastry.Categories.Add(categoryList.FirstOrDefault(c => c.ID == i));
                }
            }

            RestClient<Address> addressRC = new RestClient<Address>();
            pastry.Address = await addressRC.PostAsync(pastry.Address);
            if (pastry.Address != null)
            {
                pastry.Address_FK = pastry.Address.ID;
                pastry.Address = null;
                PastryShopRestClient pastryShopRC = new PastryShopRestClient();
                pastry = await pastryShopRC.PostAsync(pastry);

                if (pastry == null)
                {
                    await DisplayAlert("Erreur", "Erreur lors de l'enregistrement des informations!", "Ok");
                    return;
                }
                else
                {
                    RestClient<PastryDeleveryPayment> pastryDeleveryPaymentRC = new RestClient<PastryDeleveryPayment>();
                    foreach (PastryShopDeleveryMethod pastryShopDM in PSdeleveryMethods)
                    {
                        pastryShopDM.PastryShop_FK = pastry.ID;
                        List<PastryDeleveryPayment> pastryDeleveryPayments = pastryShopDM.PastryDeleveryPayments.ToList();
                        pastryShopDM.PastryDeleveryPayments.Clear();
                        RestClient<PastryShopDeleveryMethod> pastryShopDeleveryMethodRC = new RestClient<PastryShopDeleveryMethod>();
                        PastryShopDeleveryMethod PSDM = await pastryShopDeleveryMethodRC.PostAsync(pastryShopDM);
                        foreach (PastryDeleveryPayment p in pastryDeleveryPayments)
                        {
                            //p.PastryShopDeleveryMethods.Add(pastryShopDM);
                            p.PastryShopDeleveryMethod_FK = PSDM.ID;
                            await pastryDeleveryPaymentRC.PostAsync(p);
                        }
                    }
                    await Navigation.PushAsync(new PastryShopEnteringMenu(pastry));
                }
            }
        }

        private void CategorySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if ((sender as Switch).IsToggled)
            {
                categorySwitchs.Add(Int32.Parse((sender as Switch).ClassId));
            }
            else
            {
                categorySwitchs.Remove(Int32.Parse((sender as Switch).ClassId));
            }
        }

        private void DelayPicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer")
            {
                (sender as Picker).SelectedIndex = 0;
            }
        }

        private void DelayPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var s = sender;
            if ((sender as Picker).SelectedIndex >= 0)
            {
                int Index = Int32.Parse((((sender as Picker).Parent as StackLayout).Children[0] as Picker).ClassId);
                int x = (sender as Picker).SelectedIndex;
                localDeleveryMethods.FirstOrDefault(l => l.Index == Index).selectedDeleveryDelay = deleveryDelays.ElementAt(x);
            }
        }

        private void PaymentMethodToggeled(object sender, ToggledEventArgs e)
        {
            int index = Int32.Parse(((((((sender as Switch).Parent as Grid).Parent as StackLayout).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Picker).ClassId);
            deleveryMethodLocal localD = localDeleveryMethods.FirstOrDefault(d => d.Index == index);
            if ((sender as Switch).IsToggled)
            {
                localD.selectedPayments.Add(localD.selectedDeleveryMethod.Payments.FirstOrDefault(p => p.ID == Int32.Parse((sender as Switch).ClassId)));
            }
            else
            {
                localD.selectedPayments.Remove(localD.selectedDeleveryMethod.Payments.FirstOrDefault(p => p.ID == Int32.Parse((sender as Switch).ClassId)));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
