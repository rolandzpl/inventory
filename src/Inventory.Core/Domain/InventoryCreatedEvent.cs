using System;

namespace Inventory.Domain
{
    public class InventoryCreatedEvent : Event
    {
        public InventoryCreatedEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
