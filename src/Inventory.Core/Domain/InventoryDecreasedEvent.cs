namespace Inventory.Domain
{
    public class InventoryDecreasedEvent : Event
    {
        public InventoryDecreasedEvent(int amount)
        {
            Amount = amount;
        }

        public int Amount { get; }
    }
}