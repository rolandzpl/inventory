using System;
using System.Collections.Generic;

namespace DDD.Domain
{
	internal class DelegatedEventStore<TId> : IEventStore
	{
		private Func<TId, IEnumerable<Event>> getEventsById;
		private Action<IEnumerable<Event>> persistEvents;

		public DelegatedEventStore(
			Func<TId, IEnumerable<Event>> getEventsById,
			Action<IEnumerable<Event>> persistEvents)
		{
			this.getEventsById = getEventsById ?? throw new ArgumentNullException(nameof(getEventsById));
			this.persistEvents = persistEvents ?? throw new ArgumentNullException(nameof(persistEvents));
		}

		public IEnumerable<Event> GetEventsById(object id)
		{
			return getEventsById((TId)id);
		}

		public void SaveEvents(IEnumerable<Event> events)
		{
			persistEvents(events);
		}
	}
}