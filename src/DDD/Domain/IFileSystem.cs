using System.IO;

namespace DDD.Domain
{
	public interface IFileSystem
	{
		TextWriter CreateText(string path);
	}
}