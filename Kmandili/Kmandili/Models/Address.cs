using System;
using System.Collections.Generic;

namespace Kmandili.Models
{
    public class Address
    {
        public Address()
        {
            this.PastryShops = new HashSet<PastryShop>();
            this.PointOfSales = new HashSet<PointOfSale>();
            this.Users = new HashSet<User>();
        }

        public int ID { get; set; }
        public Nullable<int> Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public Nullable<int> ZipCode { get; set; }

        public virtual ICollection<PastryShop> PastryShops { get; set; }
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public override string ToString()
        {
            return Number + ", Rue " + Street + ", " + City + ", " + State + ", " + Country;
        }
    }
}
