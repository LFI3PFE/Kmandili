using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilterPopupPage : PopupPage
	{
		public FilterPopupPage (Page page, StackLayout content)
		{
            BackgroundColor = Color.FromHex("#CC000000");
            InitializeComponent ();
		    this.Content = content;
		}
	}
}
