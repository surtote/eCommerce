namespace Orders.Models
{
    public class OrderProduct
    {
        public Guid Id { get; set; }

        // FK hacia Order
        public Guid OrderId { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public Order Order { get; set; }
        public Guid ProductId { get; set; }
    }
}
