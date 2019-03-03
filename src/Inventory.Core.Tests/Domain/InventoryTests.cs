using NUnit.Framework;
using System.Collections.Generic;

namespace Inventory.Domain
{
    class InventoryTests
    {
        [Test]
        public void InventoryCreated_Always_EmitsInventoryCreatedEvent()
        {
            var inventory = Inventory.Create();
            IEnumerable<Event> changes = inventory.GetUncommittedChanges();
            Assert.That(changes, 
                Has.Exactly(1)
                    .With.TypeOf<InventoryCreatedEvent>());
        }

        [Test]
        public void IncreaseInventory_ByAmountOfN_EmitsInventoryIncreasedEvent()
        {
            var N = 1;
            inventory.Increase(N);
            Assert.That(
                inventory.GetUncommittedChanges(),
                Has.Exactly(1)
                    .TypeOf<InventoryIncreasedEvent>()
                    .And.Matches<InventoryIncreasedEvent>(x => x.Amount == N));
        }

        [Test]
        public void DecreaseInventory_ByAmountThetIsAvailable_EmitsInventoryDecreasedEvent()
        {
            var N = 1;
            inventory.Increase(N);
            inventory.Decrease(N);
            Assert.That(
                inventory.GetUncommittedChanges(),
                Has.Exactly(1)
                    .TypeOf<InventoryDecreasedEvent>()
                    .And.Matches<InventoryDecreasedEvent>(e => e.Amount == N));
        }

        [Test]
        public void DecreaseInventory_ByAmountGreaterThanActuallyAvailable_ThrowsException()
        {
            var N = 1;
            Assert.Throws<InvalidAmountException>(() => inventory.Decrease(N));
        }

        [SetUp]
        protected  void SetUp()
        {
            inventory = Inventory.Create();
        }

        private Inventory inventory;
    }
}
