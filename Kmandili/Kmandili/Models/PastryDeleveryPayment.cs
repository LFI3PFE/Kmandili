namespace Kmandili.Models
{
    public class PastryDeleveryPayment
    {
        public int ID { get; set; }
        public int PastryShopDeleveryMethod_FK { get; set; }
        public int Payment_FK { get; set; }

        public virtual PastryShopDeleveryMethod PastryShopDeleveryMethod { get; set; }
        public virtual Payment Payment { get; set; }

        public override string ToString()
        {
            return Payment.PaymentMethod;
        }
    }
}
