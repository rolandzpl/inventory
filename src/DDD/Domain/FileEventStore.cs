using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DDD.Domain
{
	public class FileEventStore : IEventStore
	{
		private readonly string rootDirectory;
		private readonly IFileSystem fs;
		private readonly IEventSerializer serializer;
		private readonly JsonSerializer eventSerializer;

		public FileEventStore(string rootDirectory, IFileSystem fs, IEventSerializer serializer)
		{
			this.fs = fs ?? throw new ArgumentNullException(nameof(fs));
			this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			this.eventSerializer = JsonSerializer.CreateDefault();
		}

		public IEnumerable<Event> GetEventsById(object id)
		{
			return GetEventDataForId(id)
				.OrderBy(ed => ed.AggregateVersion)
				.Select(ed => serializer.Deserialize(ed.Payload, GetEventType(ed.EventName)))
				.ToList();
		}

		private Type GetEventType(string eventName)
		{
			return Type.GetType(eventName);
		}

		private IEnumerable<EventData> GetEventDataForId(object id)
		{
			var files = fs.GetFiles($"{id}-*.event");
			foreach (var f in files)
			{
				using (var reader = fs.OpenText(f))
				using (var json = new JsonTextReader(reader))
				{
					yield return eventSerializer.Deserialize<EventData>(json);
				}
			}
		}

		public void SaveEvents(object id, IEnumerable<Event> events, int expectedVersion)
		{
			if (expectedVersion <= GetMaxVersion(id))
			{
				throw new ConcurrencyException();
			}
			foreach (var e in events)
			{
				var path = $"{id}-{e.Version}.event";
				var currentVersion = expectedVersion;
				using (TextWriter writer = fs.CreateText(path))
				{
					eventSerializer.Serialize(writer, new EventData()
					{
						Timestamp = DateTime.UtcNow,
						EventId = Guid.NewGuid(),
						EventName = e.GetType().Name,
						AggregateId = id,
						AggregateVersion = currentVersion++,
						Payload = serializer.Serialize(e)
					});
					writer.Flush();
				}
			}
		}

		private int GetMaxVersion(object id)
		{
			return GetEventDataForId(id)
				.Select(ed => ed.AggregateVersion)
				.DefaultIfEmpty(int.MinValue)
				.Max();
		}
	}

	public class EventData
	{
		public DateTime Timestamp;
		public Guid EventId;
		public object AggregateId;
		public int AggregateVersion;
		public string Payload;
		public string EventName;
	}
}
