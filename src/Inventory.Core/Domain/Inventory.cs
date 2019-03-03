using System.Collections.Generic;
using System.Linq;

namespace Inventory.Domain
{
    public class Inventory
    {
        private readonly List<Event> changes = new List<Event>();
        private int actualAmount;

        public int Amount
        {
            get { return actualAmount; }
        }

        public static Inventory Create()
        {
            return new Inventory();
        }

        public static Inventory LoadFrom(IEnumerable<Event> history)
        {
            var instance = new Inventory();
            return instance;
        }

        public Inventory()
        {
            var e = new InventoryCreatedEvent();
            ApplyNewEvent(e);
        }

        private void ApplyNewEvent(InventoryCreatedEvent e)
        {
            changes.Add(e);
            Apply(e);
        }

        private void Apply(InventoryCreatedEvent e) { }

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
