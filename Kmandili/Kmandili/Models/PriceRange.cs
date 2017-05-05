using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class PriceRange
    {
        public PriceRange()
        {
            this.PastryShops = new HashSet<PastryShop>();
        }

        public int ID { get; set; }
        public double MinPriceRange { get; set; }
        public double MaxPriceRange { get; set; }
        
        public virtual ICollection<PastryShop> PastryShops { get; set; }

        public override string ToString()
        {
            return MinPriceRange + "-" + MaxPriceRange + " TND";
        }
    }
}
