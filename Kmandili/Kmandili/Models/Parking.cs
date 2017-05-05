using System;
using System.Collections.Generic;
using System.Text;

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
