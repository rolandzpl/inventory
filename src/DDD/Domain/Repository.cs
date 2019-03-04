using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDD.Domain
{
    public class Repository<T> where T : AggregateRoot
    {
        private readonly Func<Guid, IEnumerable<Event>> historyProvider;
        private readonly Action<IEnumerable<Event>> persistEvents;

        public Repository(
            Func<Guid, IEnumerable<Event>> historyProvider,
            Action<IEnumerable<Event>> persistEvents)
        {
            this.historyProvider = historyProvider ?? throw new ArgumentNullException(nameof(historyProvider));
            this.persistEvents = persistEvents ?? throw new ArgumentNullException(nameof(persistEvents));
        }

        public T GetItemById(Guid id)
        {
            var history = historyProvider.Invoke(id);
            if (!history.Any())
            {
                throw new KeyNotFoundException($"Instance with id {id} was not found");
            }
            var ctor = GetConstructor();
            var instance = (T)ctor.Invoke(null);
            foreach (var e in history)
            {
                instance.ApplyEvent(e);
            }
            return instance;
        }

        private static ConstructorInfo GetConstructor()
        {
            return typeof(T)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == 0)
                .FirstOrDefault();
        }

        public void Save(T item)
        {
            persistEvents.Invoke(item.GetUncommittedChanges());
            item.ClearUncommittedChanges();
        }
    }
}
