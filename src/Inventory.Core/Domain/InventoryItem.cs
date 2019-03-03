using System.Collections.Generic;
using System.Linq;

namespace Inventory.Domain
{
    public class Inventory
    {
        private readonly List<Event> changes = new List<Event>();
        private int actualAmount;

        public static Inventory Create()
        {
            return new Inventory();
        }

        public Inventory()
        {
            changes.Add(new InventoryCreatedEvent());
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return changes.ToList();
        }

        public void Increase(int amount)
        {
            changes.Add(new InventoryIncreasedEvent(amount));
            actualAmount++;
        }

        public void Decrease(int amount)
        {
            if (actualAmount < amount)
            {
                throw new InvalidAmountException();
            }
            changes.Add(new InventoryDecreasedEvent(amount));
            actualAmount--;
        }
    }
}
