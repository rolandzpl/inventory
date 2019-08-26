using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Domain
{
	public interface IEventStore
	{
		void SaveEvents(object id, IEnumerable<Event> events, int expectedVersion);

		IEnumerable<Event> GetEventsById(object id);

		event EventHandler<NewEventsEventArgs> NewEvents;
	}

	public class NewEventsEventArgs : EventArgs
	{
		public NewEventsEventArgs(IEnumerable<Event> events)
		{
			NewEvents = events.ToList();
		}

		public List<Event> NewEvents { get; }
	}
}
