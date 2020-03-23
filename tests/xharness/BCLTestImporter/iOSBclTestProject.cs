using System;
using System.Collections.Generic;

namespace Xharness.BCLTestImporter {
	public class iOSBclTestProject {
		public string Name;
		public string Path;
		public bool XUnit;
		public string ExtraArgs;
		public List<Platform> Platforms;
		public string Failure;
		public double TimeoutMultiplier = 1;
	}
}
