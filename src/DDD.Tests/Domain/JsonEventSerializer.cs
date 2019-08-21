using Newtonsoft.Json;
using System.IO;

namespace DDD.Domain
{
	public class JsonEventSerializer : IEventSerializer
	{
		private readonly JsonSerializer serializer;

		public JsonEventSerializer()
		{
			this.serializer = JsonSerializer.CreateDefault();
		}

		public string Serialize(Event e)
		{
			var writer = new StringWriter();
			serializer.Serialize(writer, e);
			return writer.GetStringBuilder().ToString();
		}
	}
}