using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Parking
    {
        public Parking()
        {
            this.PointOfSales = new HashSet<PointOfSale>();
        }

        public int ID { get; set; }
        public string ParkingType { get; set; }
        
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }

        public override string ToString()
        {
            return ParkingType;
        }
    }
}
