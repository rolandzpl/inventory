using DDD.Fakes;
using NUnit.Framework;
using System;
using static DDD.Fakes.FakeFileSystem;

namespace DDD.Domain
{
	class FileEventStoreTests
	{
		[Test]
		public void CreateEventStore_NullFileSystem_ThrowsException()
		{
			Assert.Multiple(() =>
			{
				Assert.Throws<ArgumentNullException>(() => new FileEventStore(null, new JsonEventSerializer()));
				Assert.Throws<ArgumentNullException>(() => new FileEventStore(new FakeFileSystem(), null));
			});
		}

		[Test]
		public void SaveEvents_()
		{
			var fileSystem = new FakeFileSystem();
			var serializer = new JsonEventSerializer();
			var eventStore = new FileEventStore(fileSystem, serializer);
			var aggregateId = Guid.NewGuid();
			var events = new Event[]
			{
				new TestDomainCreatedEvent(aggregateId)
			};
			eventStore.SaveEvents(aggregateId, events, -1);
			Assert.That(
				fileSystem.Files,
				Has.Exactly(1).With.Matches<File>(f => f.Path == $"{aggregateId}.json"));
		}
	}
}
