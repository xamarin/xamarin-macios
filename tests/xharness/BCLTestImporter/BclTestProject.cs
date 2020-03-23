using System;
namespace Xharness.BCLTestImporter {
	public class BclTestProject {
		public string Name;
		public string Path;
		public bool XUnit;
		public string ExtraArgs;
		public string Failure;
		public double TimeoutMultiplier = 1;
	}
}
