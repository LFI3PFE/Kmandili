using System;
using System.Collections.Generic;
using System.Linq;

namespace Kmandili.Models
{
    public class PastryShop
    {
        public PastryShop()
        {
            this.Orders = new HashSet<Order>();
            this.PastryShopDeleveryMethods = new HashSet<PastryShopDeleveryMethod>();
            this.PointOfSales = new HashSet<PointOfSale>();
            this.Products = new HashSet<Product>();
            this.Ratings = new HashSet<Rating>();
            this.Categories = new HashSet<Category>();
            this.PhoneNumbers = new HashSet<PhoneNumber>();
            JoinDate = DateTime.Now;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string ProfilePic { get; set; }
        public string CoverPic { get; set; }
        public int PriceRange_FK { get; set; }
        public int Address_FK { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public System.DateTime JoinDate { get; set; }
        public double Rating
        {
            get { return Ratings.Count == 0 ? 0 : Math.Round(((double)Ratings.Sum(r => r.Value) / Ratings.Count), 1); }
        }

        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual PriceRange PriceRange { get; set; }
        public virtual ICollection<PastryShopDeleveryMethod> PastryShopDeleveryMethods { get; set; }
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }
}
