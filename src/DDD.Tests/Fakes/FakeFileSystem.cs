using DDD.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DDD.Fakes
{
	public class FakeFileSystem : IFileSystem
	{
		private List<File> files = new List<File>();

		public IEnumerable<File> Files
		{
			get { return files.ToList(); }
		}

		public TextWriter CreateText(string path)
		{
			throw new NotImplementedException();
		}

		public class File
		{
			public string Path { get; private set; }
		}
	}
}