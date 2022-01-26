using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ObsoleteTest {

		[TestCaseSource (typeof (Helper), "NetPlatformAssemblies")] // call this method with every .net6 library
		public void GetAllObsoletedThings (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}
			Console.WriteLine (assemblyPath);

			// Make a list of Obsolete things
			HashSet<string> found = new HashSet<string> ();

			foreach (var prop in Helper.FilterProperties (assembly, a => HasObsoleteAttribute (a))) {
				if (Skip (prop))
					continue;
				found.Add (prop.FullName);
			}

			foreach (var meth in Helper.FilterMethods (assembly, a => HasObsoleteAttribute (a))) {
				if (Skip (meth))
					continue;
				found.Add (meth.FullName);
			}

			foreach (var type in Helper.FilterTypes (assembly, a => HasObsoleteAttribute (a))) {
				if (Skip (type))
					continue;
				found.Add (type.FullName);
			}

			// TODO: Events?

			Assert.That (found, Is.Empty, string.Join (Environment.NewLine, found));
		}

		bool HasObsoleteAttribute (ICustomAttributeProvider provider) => HasObsoleteAttribute (provider.CustomAttributes);

		bool HasObsoleteAttribute (IEnumerable<CustomAttribute> attributes) => attributes.Any (a => IsObsoleteAttribute (a));

		bool IsObsoleteAttribute (CustomAttribute attribute) => attribute.AttributeType.Name == "Obsolete" || (attribute.AttributeType.Name == "ObsoleteAttribute");

		bool Skip (MemberReference member)
		{
			var ns = member.FullName.Split ('.') [0];

			// Skipping all namespaces until issue https://github.com/xamarin/xamarin-macios/issues/13621 is fixed
			switch (ns) {
			default:
				return true;
			}
		}
	}
}
