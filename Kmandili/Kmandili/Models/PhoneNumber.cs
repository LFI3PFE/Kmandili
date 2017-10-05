namespace Kmandili.Models
{
    public class PhoneNumber
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public int PhoneNumberType_FK { get; set; }

        public virtual PhoneNumberType PhoneNumberType { get; set; }
        public virtual PastryShop PastryShop { get; set; }
        public virtual PointOfSale PointOfSale { get; set; }
        public virtual User User { get; set; }
    }
}
