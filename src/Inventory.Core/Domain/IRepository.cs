using System.Collections.Generic;
using DDD.Domain;

namespace Inventory.Domain
{
	public interface IRepository
	{
		void Save(IEnumerable<Event> enumerable);
	}
}