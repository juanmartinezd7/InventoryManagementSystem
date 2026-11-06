// Represents a single stock movement.
public struct InventoryTransaction
{
    public int ProductId { get; set; }
    public int QuantityChanged { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
}