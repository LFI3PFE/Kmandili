using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kmandili.Interfaces;
using Kmandili.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace Kmandili.UWP
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "ms-appx-web:///";
        }
    }
}
