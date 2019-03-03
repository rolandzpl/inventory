using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.Domain
{
    class RepositoryTests
    {
        [Test]
        public void GetItemById_GivenHistory_ReturnsInstantInFinalState()
        {
            var id = Guid.NewGuid();
            var repository = new Repository<Inventory>(
                _ => new Event[] {
                    new InventoryCreatedEvent(id),
                    new InventoryIncreasedEvent(10),
                    new InventoryDecreasedEvent(2)
                },
                _ => { });
            var inventory = repository.GetItemById(id);
            Assert.That(inventory.Amount, Is.EqualTo(8));
        }

        [Test]
        public void Save_GivenHistory_ReturnsInstantInFinalState()
        {
            var savedItems = new List<Event>();
            var repository = new Repository<Inventory>(_ => Enumerable.Empty<Event>(), _ => savedItems.AddRange(_));
            repository.Save(Inventory.Create());
            Assert.That(savedItems, Is.Not.Empty);
        }

        [Test]
        public void Save_Successfull_UncommittedChangesAreEmpty()
        {
            var inventory = Inventory.Create();
            var repository = new Repository<Inventory>(_ => Enumerable.Empty<Event>(), _ => { });
            repository.Save(inventory);
            Assert.That(inventory.GetUncommittedChanges(), Is.Empty);
        }

        [Test]
        public void Save_OnError_AllChangesRemainInInstance()
        {
            var inventory = Inventory.Create();
            var repository = new Repository<Inventory>(_ => Enumerable.Empty<Event>(), _ => throw new Exception());
            try { repository.Save(inventory); } catch { }
            Assert.That(inventory.GetUncommittedChanges(), Is.Not.Empty);
        }
    }
}
