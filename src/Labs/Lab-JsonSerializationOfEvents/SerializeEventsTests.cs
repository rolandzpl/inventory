using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Inventory
{
	class SerializeEventsTests
	{
		[Test]
		public void x()
		{
			var serializer = JsonSerializer.CreateDefault();
			var events = new Event[]
			{
				new EmployeeCreatedEvent() { Version = 0, EmployeeId = Guid.NewGuid(), FirstName = "Bill", LastName = "Gates" },
				new EmployeeCreatedEvent() { Version = 0, EmployeeId = Guid.NewGuid(), FirstName = "Steve", LastName = "Jobs" },
				new EmployeeCreatedEvent() { Version = 0, EmployeeId = Guid.NewGuid(), FirstName = "Steve", LastName = "Wozniak" },
				new EmployeeCreatedEvent() { Version = 0, EmployeeId = Guid.NewGuid(), FirstName = "Sergey", LastName = "Brinn" },
				new EmployeeCreatedEvent() { Version = 0, EmployeeId = Guid.NewGuid(), FirstName = "Larry", LastName = "Page" },
			};
			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			foreach (var e in events)
			{
				using (var jsonWriter = new StringWriter())
				using (var json = new JsonTextWriter(jsonWriter))
				{
					serializer.Serialize(json, e);
					writer.WriteLine(jsonWriter.GetStringBuilder().ToString());
				}
			}
			var data = sb.ToString();

			var reader = new StringReader(data);
		}
	}

	public class Event
	{
		public int Version { get; set; }
	}

	public class EmployeeCreatedEvent : Event
	{
		public Guid EmployeeId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}