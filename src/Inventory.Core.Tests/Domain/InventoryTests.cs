using NUnit.Framework;
using System.Collections.Generic;

namespace Inventory.Domain
{
    class InventoryTests
    {
        [Test]
        public void InventoryCreated_Always_EmitsInventoryCreatedEvent()
        {
            Inventory inventory = Inventory.Create();
            IEnumerable<Event> changes = inventory.GetUncommittedChanges();
            Assert.That(changes, Has.Exactly(1).InstanceOf<InventoryCreatedEvent>());
        }

        [Test]
        public void IncreaseInventory_ByAmount_EmitsInventoryIncreasedEvent()
        {
            var inventory = Inventory.Create();
            inventory.Increase(1);
            Assert.That(
                inventory.GetUncommittedChanges(),
                Has.Exactly(1).InstanceOf<InventoryIncreasedEvent>());
        }

        [Test]
        public void DecreaseInventory_ByAmount_EmitsInventoryDecreasedEvent()
        {
            var inventory = Inventory.Create();
            inventory.Decrease(1);
            Assert.That(
                inventory.GetUncommittedChanges(), 
                Has.Exactly(1).InstanceOf<InventoryDecreasedEvent>());
        }
    }
}
