using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ObsoleteTest {

		[TestCaseSource (typeof (Helper), "Net6PlatformAssemblies")] // call this method with every .net6 library
		public void GetAllObsoletedThings (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			Console.WriteLine(assemblyPath);

			// Make a list of Obsolete things
			HashSet<string> found = new HashSet<string> ();

			foreach (var prop in Helper.FilterProperties (assembly, a => HasObsoleteAttribute (a))) {
				if (!ShouldTest (prop))
					continue;
				found.Add (prop.FullName);
			}

			foreach (var meth in Helper.FilterMethods (assembly, a => HasObsoleteAttribute (a))) {
				if (!ShouldTest (meth))
					continue;
				found.Add (meth.FullName);
			}

			foreach (var type in Helper.FilterTypes (assembly, a => HasObsoleteAttribute (a))) {
				if (!ShouldTest (type))
					continue;
				found.Add (type.FullName);
			}

			// TODO: Events?

			Console.WriteLine (found.Count);
			Assert.That (found, Is.Empty, string.Join (Environment.NewLine, found));
		}

		bool HasObsoleteAttribute (PropertyDefinition prop) => HasObsoleteAttribute (prop.CustomAttributes);
		bool HasObsoleteAttribute (MethodDefinition meth) => HasObsoleteAttribute (meth.CustomAttributes);
		bool HasObsoleteAttribute (TypeDefinition type) => HasObsoleteAttribute (type.CustomAttributes);

		bool HasObsoleteAttribute (IEnumerable<CustomAttribute> attributes) => attributes.Any (a => IsObsoleteAttribute (a));

		bool IsObsoleteAttribute (CustomAttribute attribute)
		{
			return attribute.AttributeType.Name == "Obsolete" ||
				(attribute.AttributeType.Name == "ObsoleteAttribute");
		}

		bool ShouldTest (MemberReference member)
		{
			var ns = member.FullName.Split ('.') [0];

			switch (ns) {
			//case "Metal":
			//	return false;
			default:
				return true;
			}
		}
	}
}
