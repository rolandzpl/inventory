using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inventory.Domain
{
    public class Inventory
    {
        private Guid id;
        private readonly List<Event> changes = new List<Event>();
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

        private void ApplyNewEvent(Event e)
        {
            changes.Add(e);
            ApplyEvent(e);
        }

        private void ApplyEvent(Event e)
        {
            var handler = GetType()
                .GetRuntimeMethods()
                .Where(mi => mi.IsPrivate)
                .Where(mi => mi.Name == "Apply")
                .Where(mi => mi.GetParameters().Length == 1)
                .SingleOrDefault(mi => mi.GetParameters().SingleOrDefault()?.ParameterType == e.GetType());
            handler?.Invoke(this, new[] { e });
        }

        private void Apply(InventoryCreatedEvent e)
        {
            this.id = e.Id;
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return changes.ToList();
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
