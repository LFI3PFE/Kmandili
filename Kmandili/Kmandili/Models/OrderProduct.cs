namespace Kmandili.Models
{
    public class OrderProduct
    {
        public int Order_FK { get; set; }
        public int Product_FK { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        public double Total { get { 
            if (Product != null)
            {
                    return (Product.Price*Quantity);
            }
            return 0;
        } }
    }
}
