using System;
using System.Collections.Generic;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class EnumTest {

		[TestCaseSource (typeof (Helper), "PlatformAssemblies")]
		// https://github.com/xamarin/xamarin-macios/issues/9724
		public void NoAvailabilityOnError (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			HashSet<string> found = new HashSet<string> ();
			foreach (var type in assembly.MainModule.Types)
				NoAvailabilityOnError (type, found);

			Assert.That (found, Is.Empty, string.Join (", ", found));
		}

		bool IsErrorEnum (TypeDefinition type)
		{
			if (!type.IsEnum)
				return false;

			if (type.Name.EndsWith ("Error", StringComparison.Ordinal))
				return true;
			if (type.Name.EndsWith ("ErrorCode", StringComparison.Ordinal))
				return true;

			if (!type.HasCustomAttributes)
				return false;
			foreach (var ca in type.CustomAttributes) {
				if (ca.AttributeType.Name == "ErrorDomainAttribute")
					return true;
			}
			return false;
		}

		void NoAvailabilityOnError (TypeDefinition type, ISet<string> found)
		{
			if (type.HasNestedTypes) {
				foreach (var nt in type.NestedTypes)
					NoAvailabilityOnError (nt, found);
			}

			if (!IsErrorEnum (type))
				return;

			foreach (var f in type.Fields) {
				if (!f.HasCustomAttributes)
					continue;
				foreach (var ca in f.CustomAttributes) {
					switch (ca.AttributeType.Name) {
					case "ObsoleteAttribute":
						// this is fine
						break;
					case "IntroducedAttribute":
					case "UnavailableAttribute":
					case "iOSAttribute":
					case "MacAttribute":
					case "TVAttribute":
					case "WatchAttribute":
					case "NoiOSAttribute":
					case "NoMacAttribute":
					case "NoTVAttribute":
					case "NoWatchAttribute":
						found.Add ($"{type.FullName}.{f.Name}");
						break;
					default:
						break;
					}
				}
			}
		}
	}
}
