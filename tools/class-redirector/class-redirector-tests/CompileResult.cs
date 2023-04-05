using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ClassRedirectorTests {
	public class CompileResult : IDisposable
	{
		bool disposedValue;

		public CompileResult(string directoryPath, string outputFileName, string error)
		{
			DirectoryPath = directoryPath;
			OutputFileName = outputFileName;
			Error = error;
		}

		public string DirectoryPath { get; private set; }
		public string OutputFileName { get; private set; }
		public string Error { get; private set; }

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					if (Directory.Exists (DirectoryPath)) {
						DeleteDirectoryAndContents (DirectoryPath);
					}
				}
				disposedValue = true;
			}
		}

		public void Dispose ()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose (disposing: true);
			GC.SuppressFinalize (this);
		}

		static void DeleteDirectoryAndContents (string path)
		{
			var directories = new Stack<string> ();
			directories.Push (path);
			while (directories.Count > 0) {
				var currDirectory = directories.Pop ();
				PushAll (directories, Directory.EnumerateDirectories (currDirectory));
				foreach (var file in Directory.EnumerateFiles (currDirectory))
					File.Delete (file);
				Directory.Delete (currDirectory);
			}
		}

		static void PushAll<T> (Stack<T> stack, IEnumerable<T> items)
		{
			foreach (var item in items)
				stack.Push (item);
		}
	}
}

