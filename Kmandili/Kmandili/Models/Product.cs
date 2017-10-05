using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Product
    {
        public Product()
        {
            this.OrderProducts = new HashSet<OrderProduct>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pic { get; set; }
        public double Price { get; set; }
        public int SaleUnit_FK { get; set; }
        public int Category_FK { get; set; }
        public int PastryShop_FK { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual PastryShop PastryShop { get; set; }
        public virtual SaleUnit SaleUnit { get; set; }
    }
}
