namespace Shared.Events;

public class InventoryReleasedEvent
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}