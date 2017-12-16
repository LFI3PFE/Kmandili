namespace Kmandili.Models.LocalModels
{
    public class CartProduct
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }

        public void UpdateTotal()
        {
            Total = Quantity * Product.Price;
        }
    }
}
