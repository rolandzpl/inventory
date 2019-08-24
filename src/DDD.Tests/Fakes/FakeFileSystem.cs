using DDD.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

		public IEnumerable<string> GetFiles(string path)
		{
			var pattern = path.Replace("*", ".+");
			return files
				.Where(f => Regex.Match(f.Path, pattern).Success)
				.Select(f => f.Path)
				.ToList();
		}

		public TextReader OpenText(string path)
		{
			var file = files.FirstOrDefault(f => Regex.Match(f.Path, path).Success);
			return new StringReader(file.GetStringBuilder().ToString());
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