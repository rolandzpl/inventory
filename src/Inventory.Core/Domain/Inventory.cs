using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.Domain
{
    public class Inventory : AggregateRoot
    {
        private int actualAmount;

        public int Amount
        {
            get { return actualAmount; }
        }

        public static Inventory Create()
        {
            return new Inventory(Guid.NewGuid());
        }

        public static Inventory LoadFrom(IEnumerable<Event> history)
        {
            var instance = new Inventory();
            foreach (var e in history)
            {
                instance.ApplyEvent(e);
            }
            return instance;
        }

        private Inventory(Guid id)
        {
            ApplyNewEvent(new InventoryCreatedEvent(id));
        }

        private Inventory() { }

        private void Apply(InventoryCreatedEvent e)
        {
            this.id = e.Id;
        }

        public void Increase(int amount)
        {
            ApplyNewEvent(new InventoryIncreasedEvent(amount));
        }

        private void Apply(InventoryIncreasedEvent e)
        {
            actualAmount += e.Amount;
        }

        public void Decrease(int amount)
        {
            if (actualAmount < amount)
            {
                throw new InvalidAmountException();
            }
            ApplyNewEvent(new InventoryDecreasedEvent(amount));
        }

        private void Apply(InventoryDecreasedEvent e)
        {
            actualAmount -= e.Amount;
        }
    }
}
