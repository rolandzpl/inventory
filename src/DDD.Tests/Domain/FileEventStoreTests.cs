using DDD.Fakes;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using StringReader = System.IO.StringReader;

namespace DDD.Domain
{
	class FileEventStoreTests
	{
		class Creating
		{
			[Test]
			public void CreateEventStore_NullFileSystem_ThrowsException()
			{
				Assert.Multiple(() =>
				{
					Assert.Throws<ArgumentNullException>(() => new FileEventStore("", null, new JsonEventSerializer()));
					Assert.Throws<ArgumentNullException>(() => new FileEventStore("", new FakeFileSystem(), null));
				});
			}
		}

		class SavingOrReading
		{
			[Test]
			public void SaveEvents_NoConcurrencyConflict_NewEventFileCreated()
			{
				var events = GetEvents();
				eventStore.SaveEvents(aggregateId, events, -1);
				Assert.That(fileSystem.Files.Count(), Is.EqualTo(1));
			}

			private Event[] GetEvents()
			{
				return new Event[]
				{
					new TestDomainCreatedEvent(aggregateId)
				};
			}

			[Test]
			public void SaveEvents_NoConcurrencyConflict_EventDataStored()
			{
				var events = GetEvents();
				eventStore.SaveEvents(aggregateId, events, -1);
				Assert.That(
					fileSystem.Files.Select(f => DeserializeEvent(f.GetStringBuilder())),
					Has.One.Matches<EventData>(e =>
						e.AggregateId.Equals(aggregateId.ToString()) &&
						e.EventName == "TestDomainCreatedEvent"));
			}

			[Test]
			public void SaveEvents_EventWithHigherVersionAlreadyExists_ConcurrencyExceptionIsThrown()
			{
				StoreEventDataInFilesystem(aggregateId, 0);
				var events = GetEvents();
				Assert.Throws<ConcurrencyException>(() => eventStore.SaveEvents(aggregateId, events, -1));
			}

			private void StoreEventDataInFilesystem(Guid aggregateId, int version)
			{
				var file = fileSystem.CreateText($"{aggregateId}-0.event");
				var eventData = new EventData()
				{
					AggregateId = aggregateId,
					AggregateVersion = version
				};
				var sss = JsonSerializer.CreateDefault();
				sss.Serialize(file, eventData);
			}

			private EventData DeserializeEvent(StringBuilder sb)
			{
				var serializer = JsonSerializer.CreateDefault();
				var reader = new StringReader(sb.ToString());
				var json = new JsonTextReader(reader);
				return serializer.Deserialize<EventData>(json);
			}

			[SetUp]
			protected void SetUp()
			{
				fileSystem = new FakeFileSystem();
				serializer = new JsonEventSerializer();
				eventStore = new FileEventStore("", fileSystem, serializer);
				aggregateId = Guid.NewGuid();
			}

			private FakeFileSystem fileSystem;
			private JsonEventSerializer serializer;
			private FileEventStore eventStore;
			private Guid aggregateId;
		}
	}
}