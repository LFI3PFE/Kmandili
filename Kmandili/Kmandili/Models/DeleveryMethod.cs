using System.Collections.Generic;

namespace Kmandili.Models
{
    public class DeleveryMethod
    {
        public DeleveryMethod()
        {
            this.Orders = new HashSet<Order>();
            this.PastryShopDeleveryMethods = new HashSet<PastryShopDeleveryMethod>();
            this.Payments = new HashSet<Payment>();
        }

        public int ID { get; set; }
        public string DeleveryType { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PastryShopDeleveryMethod> PastryShopDeleveryMethods { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

        public override string ToString()
        {
            return DeleveryType;
        }
    }
}
