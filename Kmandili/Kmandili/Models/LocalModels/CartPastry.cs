using System.Collections.Generic;

namespace Kmandili.Models.LocalModels
{
    public class CartPastry
    {
        public CartPastry()
        {
            CartProducts = new List<CartProduct>();
        }

        public PastryShop PastryShop { get; set; }

        public List<CartProduct> CartProducts { get; set; }
        public double Total {
            get {
                double t = 0;
                foreach(CartProduct cartProduct in CartProducts)
                {
                    t += cartProduct.Total;
                }
                return t;
            } }
        public DeleveryMethod DeleveryMethod { get; set; }
        public Payment PaymentMethod { get; set; }
    }
}
