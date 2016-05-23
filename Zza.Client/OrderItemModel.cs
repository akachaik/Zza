namespace Zza.Client
{
    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public string ProductName { get; set; }
    }
}