using System.Collections.Generic;

namespace DDD.Domain
{
	public interface IEventStore
	{
		void SaveEvents(IEnumerable<Event> events);

		IEnumerable<Event> GetEventsById(object id);
	}
}
