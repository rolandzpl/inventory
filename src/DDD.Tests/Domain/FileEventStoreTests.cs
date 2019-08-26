using DDD.Fakes;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
			public void GetEventsById_EventsForAggregateExist_ReturnsListOfEvents()
			{
				StoreEventDataInFilesystem(aggregateId, new EventData
				{
					EventName = "TestDomainCreatedEvent",
					Payload = serializer.Serialize(
						new TestDomainCreatedEvent(aggregateId) { Version = 0 })
				});
				var events = eventStore.GetEventsById(aggregateId);
				Assert.That(events.ElementAt(0),
					Is.InstanceOf<TestDomainCreatedEvent>().And
						.Matches<TestDomainCreatedEvent>(_ =>
							_.Id == aggregateId &&
							_.Version == 0));
			}

			[Test]
			public void GetEventsById_NoEvents_ReturnsEmptyList()
			{
				var events = eventStore.GetEventsById(aggregateId);
				Assert.That(events, Is.Empty);
			}

			[Test]
			public void SaveEvents_NoConcurrencyConflict_NewEventFileCreated()
			{
				var events = GetEvents();
				eventStore.SaveEvents(aggregateId, events, -1);
				Assert.That(fileSystem.Files.Count(), Is.EqualTo(1));
			}

			[Test]
			public void SaveEvents_SeveralEvents_EveryNextEventDataHasIncrementedVersion()
			{
				var events = new Event[]
				{
					new TestDomainCreatedEvent(aggregateId),
					new TestDomainDataChangedEvent(aggregateId, "New data #1"),
					new TestDomainDataChangedEvent(aggregateId, "New data #2"),
					new TestDomainDataChangedEvent(aggregateId, "New data #3"),
					new TestDomainDataChangedEvent(aggregateId, "New data #4"),
				};
				eventStore.SaveEvents(aggregateId, events, -1);
				Assert.That(
					fileSystem.Files.Select(f => DeserializeEvent(f.GetStringBuilder())).Select(e => e.AggregateVersion),
					Is.EquivalentTo(new[] { -1, 0, 1, 2, 3 }));
			}

			[Test]
			public void SaveEvents_SeveralEvents_EveryNextEventHasIncrementedVersion()
			{
				var events = new Event[]
				{
					new TestDomainCreatedEvent(aggregateId),
					new TestDomainDataChangedEvent(aggregateId, "New data #1"),
					new TestDomainDataChangedEvent(aggregateId, "New data #2"),
					new TestDomainDataChangedEvent(aggregateId, "New data #3"),
					new TestDomainDataChangedEvent(aggregateId, "New data #4"),
				};
				eventStore.SaveEvents(aggregateId, events, -1);
				Assert.That(
					fileSystem.Files
						.Select(f => DeserializeEvent(f.GetStringBuilder()))
						.Select(ed => serializer.Deserialize(ed.Payload, GetEventType(ed.EventName)))
						.Select(e => e.Version),
					Is.EquivalentTo(new[] { -1, 0, 1, 2, 3 }));
			}

			private Type GetEventType(string eventName)
			{
				return AppDomain
					.CurrentDomain
					.GetAssemblies()
					.SelectMany(asm => GetTypesForAssembly(asm).Where(t => t.Name == eventName))
					.FirstOrDefault();
			}

			private static IEnumerable<Type> GetTypesForAssembly(System.Reflection.Assembly asm)
			{
				try
				{
					return asm.GetTypes();
				}
				catch
				{
					return Enumerable.Empty<Type>();
				}
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
				StoreEventDataInFilesystem(aggregateId,
					new EventData()
					{
						AggregateId = aggregateId,
						AggregateVersion = version
					});
			}

			private void StoreEventDataInFilesystem(Guid aggregateId, EventData eventData)
			{
				var file = fileSystem.CreateText($"{aggregateId}-0.event");
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