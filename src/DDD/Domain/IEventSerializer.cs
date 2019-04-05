namespace DDD.Domain
{
	public interface IEventSerializer
	{
		string Serialize(Event e);
	}
}