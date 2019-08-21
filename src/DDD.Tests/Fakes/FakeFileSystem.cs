using DDD.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
			var f = new File(path);
			files.Add(f);
			return new StringWriter(f.GetStringBuilder());
		}

		public class File
		{
			private readonly Lazy<StringBuilder> sb = new Lazy<StringBuilder>();
			public readonly string Path;

			public File(string path)
			{
				Path = path;
			}

			internal StringBuilder GetStringBuilder()
			{
				return sb.Value;
			}
		}
	}
}