﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kmandili.Views.Admin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdminMasterPage : ContentPage
	{
	    private AdminMasterDetailPage adminMasterDetailPage;

		public AdminMasterPage (AdminMasterDetailPage adminMasterDetailPage)
		{
			InitializeComponent ();
		    this.adminMasterDetailPage = adminMasterDetailPage;
		}

	    private void Logout(object sender, EventArgs e)
	    {
	        App.Logout();
	    }

    }
}
