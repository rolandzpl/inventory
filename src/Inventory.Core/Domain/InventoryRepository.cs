using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.Domain
{
    public class Repository<T> where T : AggregateRoot
    {
        private readonly Func<T> objectFactory;
        private readonly Func<Guid, IEnumerable<Event>> historyProvider;

        public Repository(
            Func<T> objectFactory,
            Func<Guid, IEnumerable<Event>> historyProvider)
        {
            this.objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
            this.historyProvider = historyProvider ?? throw new ArgumentNullException(nameof(historyProvider));
        }

        public T GetInventoryById(Guid id)
        {
            var history = historyProvider.Invoke(id);
            if (!history.Any())
            {
                throw new KeyNotFoundException($"Instance with id {id} was not found");
            }
            var instance = objectFactory.Invoke();
            foreach (var e in history)
            {
                instance.ApplyEvent(e);
            }
            return instance;
        }
    }
}
