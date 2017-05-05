using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
            this.PastryShops = new HashSet<PastryShop>();
        }

        public int ID { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<PastryShop> PastryShops { get; set; }

        public override string ToString()
        {
            return CategoryName;
        }
    }
}
