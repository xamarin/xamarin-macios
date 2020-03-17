using System;
namespace Xharness.BCLTestImporter {
	public class BclTestProjectInfo {
		public string Name;
		public string [] assemblies;
		public string ExtraArgs;
		public string Group;
		public double TimeoutMultiplier = 1;
	}
}
