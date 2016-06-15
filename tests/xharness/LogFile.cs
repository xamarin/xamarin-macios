using System.Collections.Generic;
using System.IO;

namespace xharness
{
	public class LogFile
	{
		public string Description;
		public string Path;

		void Write (string value)
		{
			lock (this) {
				using (var str = new FileStream (Path, FileMode.Append, FileAccess.Write, FileShare.Read)) {
					using (var writer = new StreamWriter (str))
						writer.Write (value);
				}
			}
		}

		public void WriteLine (string value)
		{
			Write (value + "\n");
		}

		public void WriteLine (string format, params object [] args)
		{
			Write (string.Format (format, args) + "\n");
		}
	}

	public class LogFiles : List<LogFile>
	{
		public LogFile Create (string directory, string filename, string name, bool overwrite = true)
		{
			var rv = new LogFile ()
			{
				Path = Path.GetFullPath (Path.Combine (directory, filename)),
				Description = name,
			};
			Add (rv);

			if (File.Exists (rv.Path)) {
				if (overwrite)
					File.Delete (rv.Path);
			} else {
				Directory.CreateDirectory (directory);
			}

			return rv;
		}
	}
}

