using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inventory.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> changes = new List<Event>();
        protected Guid id;

        protected void ApplyNewEvent(Event e)
        {
            changes.Add(e);
            ApplyEvent(e);
        }

        protected void ApplyEvent(Event e)
        {
            var handler = GetType()
                .GetRuntimeMethods()
                .Where(mi => mi.IsPrivate)
                .Where(mi => mi.Name == "Apply")
                .Where(mi => mi.GetParameters().Length == 1)
                .SingleOrDefault(mi => mi.GetParameters().SingleOrDefault()?.ParameterType == e.GetType());
            handler?.Invoke(this, new[] { e });
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return changes.ToList();
        }
    }
}