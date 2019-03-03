using System;
using System.Collections.Generic;

namespace Inventory.Domain
{
    public class Inventory
    {
        private readonly List<Event> changes = new List<Event>();

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
            return changes;
        }

        public void Increase(int amount)
        {
            changes.Add(new InventoryIncreasedEvent());
        }

        public void Decrease(int v)
        {
            changes.Add(new InventoryDecreasedEvent());
        }
    }
}
