using System;
using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderProducts = new HashSet<OrderProduct>();
            Date = DateTime.Now;
        }

        public int ID { get; set; }
        public System.DateTime Date { get; set; }
        public int Status_FK { get; set; }
        public int User_FK { get; set; }
        public int PastryShop_FK { get; set; }
        public int DeleveryMethod_FK { get; set; }
        public int PaymentMethod_FK { get; set; }
        public bool SeenUser { get; set; }
        public bool SeenPastryShop { get; set; }

        public virtual DeleveryMethod DeleveryMethod { get; set; }
        public virtual PastryShop PastryShop { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Status Status { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
