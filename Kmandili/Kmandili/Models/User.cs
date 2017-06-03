using System;
using System.Collections.Generic;
using System.Text;

namespace Kmandili.Models
{
    public class User
    {
        public User()
        {
            this.Orders = new HashSet<Order>();
            this.Ratings = new HashSet<Rating>();
            this.PhoneNumbers = new HashSet<PhoneNumber>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public System.DateTime JoindDate { get; set; }
        public int Address_FK { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }
}
