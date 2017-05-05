using System;
using System.Collections.Generic;
using System.Text;

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
            this.Categories = new HashSet<Category>();
            this.PhoneNumbers = new HashSet<PhoneNumber>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string ProfilePic { get; set; }
        public string CoverPic { get; set; }
        public int NumberOfRatings { get; set; }
        public int RatingSum { get; set; }
        public float Rating {
            get {
                if(NumberOfRatings != 0)
                {
                    return ((float)Math.Round((float)RatingSum / NumberOfRatings, 1));
                }else
                {
                    return 0;
                }
            }
            set {
                Rating = value;
            }
        }
        public int PriceRange_FK { get; set; }
        public int Address_FK { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual PriceRange PriceRange { get; set; }
        public virtual ICollection<PastryShopDeleveryMethod> PastryShopDeleveryMethods { get; set; }
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }
}
