namespace Kmandili.Models
{
    public class WorkDay
    {
        public int ID { get; set; }
        public int Day { get; set; }
        public System.TimeSpan OpenTime { get; set; }
        public System.TimeSpan CloseTime { get; set; }
        public int PointOfSale_FK { get; set; }

        public virtual PointOfSale PointOfSale { get; set; }
    }
}
