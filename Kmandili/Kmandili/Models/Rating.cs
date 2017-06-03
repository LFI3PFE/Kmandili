using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class Rating
    {
        public int User_FK { get; set; }
        public int PastryShop_FK { get; set; }
        public int Value { get; set; }

        public virtual PastryShop PastryShop { get; set; }
        public virtual User User { get; set; }
    }
}
