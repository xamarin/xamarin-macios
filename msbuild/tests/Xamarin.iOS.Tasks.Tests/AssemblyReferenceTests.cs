using System;
using System.Reflection;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class AssemblyReferenceTests
	{
		[Test]
		public void TestAssemblyReferences ()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies ();

			foreach (var assembly in assemblies) {
				if (assembly.Location.StartsWith ("/Library/Frameworks/Mono.framework/", StringComparison.OrdinalIgnoreCase))
					continue;

				if (assembly == GetType ().Assembly)
					continue;

				if (assembly.FullName.StartsWith ("nunit.", StringComparison.Ordinal))
					continue;

				if (assembly.FullName.StartsWith ("NUnitRunner", StringComparison.Ordinal))
					continue;

				switch (assembly.GetName ().Name) {
				case "Xamarin.MacDev.Tasks.Core":
				case "Xamarin.iOS.Tasks.Core":
				case "Xamarin.iOS.Tasks":
				case "Xamarin.MacDev":
					break;
				default:
					Assert.Fail ("Unknown assembly reference: {0}", assembly.FullName);
					break;
				}
			}
		}
	}
}
