using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class DeleveryDelay
    {
        public DeleveryDelay()
        {
            this.PastryShopDeleveryMethods = new HashSet<PastryShopDeleveryMethod>();
        }

        public int ID { get; set; }
        public int MinDelay { get; set; }
        public int MaxDelay { get; set; }
        
        public virtual ICollection<PastryShopDeleveryMethod> PastryShopDeleveryMethods { get; set; }

        public override string ToString()
        {
            if(MinDelay == 0 && MaxDelay == 0)
            {
                return "Instantané";
            }
            else
            {
                return MinDelay + "-" + MaxDelay + " jours";
            }
        }
    }
}
