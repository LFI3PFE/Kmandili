using System.Collections.Generic;

namespace Kmandili.Models
{
    public class PointOfSale
    {
        public PointOfSale()
        {
            this.WorkDays = new HashSet<WorkDay>();
            this.PhoneNumbers = new HashSet<PhoneNumber>();
        }

        public int ID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int PastryShop_FK { get; set; }
        public int ParkingType_FK { get; set; }
        public int Address_FK { get; set; }

        public virtual Address Address { get; set; }
        public virtual Parking Parking { get; set; }
        public virtual PastryShop PastryShop { get; set; }
        public virtual ICollection<WorkDay> WorkDays { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }
}
