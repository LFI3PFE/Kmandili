using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Kmandili.iOS;
using UIKit;
using Kmandili.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl_iOS))]
namespace Kmandili.iOS
{
    public class BaseUrl_iOS : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}