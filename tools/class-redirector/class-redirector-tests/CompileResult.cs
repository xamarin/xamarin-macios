using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ClassRedirectorTests {
	public class CompileResult {
		bool disposedValue;

		public CompileResult (string directoryPath, string outputFileName, string error)
		{
			DirectoryPath = directoryPath;
			OutputFileName = outputFileName;
			Error = error;
		}

		public string DirectoryPath { get; private set; }
		public string OutputFileName { get; private set; }
		public string Error { get; private set; }
	}
}

