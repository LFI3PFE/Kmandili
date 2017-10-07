using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadingPopupPage
    {
        public LoadingPopupPage()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("#CC000000");
            CloseWhenBackgroundIsClicked = false;
            Loading.IsRunning = true;
        }

	    protected override bool OnBackButtonPressed()
	    {
	        return true;
	    }
    }
}
