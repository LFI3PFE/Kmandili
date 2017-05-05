using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class Payment
    {
        public Payment()
        {
            this.Orders = new HashSet<Order>();
            this.PastryDeleveryPayments = new HashSet<PastryDeleveryPayment>();
            this.DeleveryMethods = new HashSet<DeleveryMethod>();
        }

        public int ID { get; set; }
        public string PaymentMethod { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PastryDeleveryPayment> PastryDeleveryPayments { get; set; }
        public virtual ICollection<DeleveryMethod> DeleveryMethods { get; set; }
    }
}
