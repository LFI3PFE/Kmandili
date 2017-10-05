using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Status
    {
        public Status()
        {
            this.Orders = new HashSet<Order>();
        }

        public int ID { get; set; }
        public string StatusName { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; }

        public override string ToString()
        {
            return StatusName;
        }
    }
}
