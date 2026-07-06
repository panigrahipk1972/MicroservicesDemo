namespace Shared.Events;

public class PaymentFailedEvent
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string Reason { get; set; } = string.Empty;
}