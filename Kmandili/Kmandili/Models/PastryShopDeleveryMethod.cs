using System.Collections.Generic;

namespace Kmandili.Models
{
    public class PastryShopDeleveryMethod
    {
        public PastryShopDeleveryMethod()
        {
            this.PastryDeleveryPayments = new HashSet<PastryDeleveryPayment>();
        }

        public int ID { get; set; }
        public int PastryShop_FK { get; set; }
        public int DeleveryMethod_FK { get; set; }
        public int DeleveryDelay_FK { get; set; }

        public virtual DeleveryDelay DeleveryDelay { get; set; }
        public virtual DeleveryMethod DeleveryMethod { get; set; }
        public virtual ICollection<PastryDeleveryPayment> PastryDeleveryPayments { get; set; }
        public virtual PastryShop PastryShop { get; set; }

        public override string ToString()
        {
            return DeleveryMethod.DeleveryType;
        }
    }
}
