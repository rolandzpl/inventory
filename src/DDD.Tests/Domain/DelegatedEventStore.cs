using System;
using System.Collections.Generic;

namespace DDD.Domain
{
	internal class DelegatedEventStore<TId> : IEventStore
	{
		private Func<object, IEnumerable<Event>> getEventsById;
		private Action<object, IEnumerable<Event>, int> persistEvents;

		public DelegatedEventStore(
			Func<object, IEnumerable<Event>> getEventsById,
			Action<object, IEnumerable<Event>, int> persistEvents)
		{
			this.getEventsById = getEventsById ?? throw new ArgumentNullException(nameof(getEventsById));
			this.persistEvents = persistEvents ?? throw new ArgumentNullException(nameof(persistEvents));
		}

		public IEnumerable<Event> GetEventsById(object id)
		{
			return getEventsById(id);
		}

		public void SaveEvents(object id, IEnumerable<Event> events, int expectedVersion)
		{
			persistEvents(id, events, expectedVersion);
		}
	}
}