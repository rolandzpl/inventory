using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDD.Domain
{
    public class Repository<TEntity, TId> where TEntity : AggregateRoot<TId>
    {
        private readonly Func<TId, IEnumerable<Event>> historyProvider;
        private readonly Action<IEnumerable<Event>> persistEvents;

        public Repository(
            Func<TId, IEnumerable<Event>> historyProvider,
            Action<IEnumerable<Event>> persistEvents)
        {
            this.historyProvider = historyProvider ?? throw new ArgumentNullException(nameof(historyProvider));
            this.persistEvents = persistEvents ?? throw new ArgumentNullException(nameof(persistEvents));
        }

        public TEntity GetItemById(TId id)
        {
            var history = historyProvider.Invoke(id);
            if (!history.Any())
            {
                throw new KeyNotFoundException($"Instance with id {id} was not found");
            }
            var ctor = GetConstructor();
            var instance = (TEntity)ctor.Invoke(null);
            foreach (var e in history)
            {
                instance.ApplyEvent(e);
            }
            return instance;
        }

        private static ConstructorInfo GetConstructor()
        {
            return typeof(TEntity)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == 0)
                .FirstOrDefault();
        }

        public void Save(TEntity item)
        {
            persistEvents.Invoke(item.GetUncommittedChanges());
            item.ClearUncommittedChanges();
        }
    }
}
