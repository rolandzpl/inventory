using DDD.Domain;

namespace Inventory.Domain
{
    public class InventoryIncreasedEvent : Event
    {
        public InventoryIncreasedEvent(int amount)
        {
            this.Amount = amount;
        }

        public int Amount { get; }
    }
}