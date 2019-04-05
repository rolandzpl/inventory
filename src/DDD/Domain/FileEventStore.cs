using System;
using System.Collections.Generic;
using System.IO;

namespace DDD.Domain
{
	public class FileEventStore : IEventStore
	{
		private readonly IFileSystem fs;
		private readonly IEventSerializer serializer;

		public FileEventStore(IFileSystem fs, IEventSerializer serializer)
		{
			this.fs = fs ?? throw new ArgumentNullException(nameof(fs));
			this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}

		public IEnumerable<Event> GetEventsById(object id)
		{
			throw new NotImplementedException();
		}

		public void SaveEvents(object id, IEnumerable<Event> events, int expectedVersion)
		{
			foreach (var e in events)
			{
				string content = serializer.Serialize(e);
				var path = $"{id}-{e.Version}.event";
				using (TextWriter writer = fs.CreateText(path))
				{
					writer.WriteLine(content);
				}
			}
		}
	}
}
