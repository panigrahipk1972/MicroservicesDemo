namespace InventoryService.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }

    public string ProductName { get; set; } = string.Empty;
}