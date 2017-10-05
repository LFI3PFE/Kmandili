using System.Collections.Generic;

namespace Kmandili.Models
{
    public class PhoneNumberType
    {
        public PhoneNumberType()
        {
            this.PhoneNumbers = new HashSet<PhoneNumber>();
        }

        public int ID { get; set; }
        public string Type { get; set; }
        
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }

        public override string ToString()
        {
            return Type;
        }
    }
}
