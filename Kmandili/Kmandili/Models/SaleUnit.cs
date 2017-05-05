using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class SaleUnit
    {
        public SaleUnit()
        {
            this.Products = new HashSet<Product>();
        }

        public int ID { get; set; }
        public string Unit { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }

        public override string ToString()
        {
            return Unit;
        }
    }
}
