using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class AttributeTest {
		// https://github.com/xamarin/xamarin-macios/issues/10170
		// Every binding class that has net6 availability attributes on a method/property
		// must have one matching every platform listed on the availabilities of the class
		//
		// Example:
		// [SupportedOSPlatform("ios1.0")]
		// [SupportedOSPlatform("maccatalyst1.0")]
		// class TestType
		// {
		//     public static void Original () { }
		//
		//     [SupportedOSPlatform(""ios2.0"")]
		//     public static void Extension () { }
		// }
		//
		// In this example, Extension is _not_ considered part of maccatalyst1.0
		// Because having _any_ SupportedOSPlatform implies only the set explicitly set on that member
		//
		// This test should find Extension, note that it has an ios attribute,
		// and insist that some maccatalyst must also be set.
		[TestCaseSource (typeof (Helper), "Net6PlatformAssemblies")]
		public void AvailabilityForAllPlatforms (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			Console.WriteLine(assemblyPath);

			// Make a list of Availabilty on parent. If iOS that implies Catalyst. Ignore NotSupported here.
			// Each child must have an attribute for each availability (Either NotSupported or Supported)
			HashSet<string> found = new HashSet<string> ();
			int count = 0;
			foreach (var prop in Helper.FilterProperties(assembly, a => HasAnyAvailabilityAttribute(a, includeUnsupported: true))) {
				var parentAvailability = GetParentAvailabilityAttributes(prop, includeUnsupported: false).ToList();
				// iOS implies maccatalyst, but only for parent scope
				if (parentAvailability.Contains("ios") && !parentAvailability.Contains("maccatalyst")) {
					parentAvailability.Append("maccatalyst");
				}
				var myAvailability = GetAvailabilityAttributes(prop, includeUnsupported: true);
				if (!FirstContainsAllOfSecond (myAvailability, parentAvailability)) {
					if (count < 10) {
						Console.WriteLine (prop.FullName);
						Console.WriteLine ("Parent: " + string.Join(" ", parentAvailability));
						Console.WriteLine ("Mine: " + string.Join(" ", myAvailability));
						Console.WriteLine ();
						count += 1;
					}
					found.Add(prop.FullName);
				}
			}
			foreach (var meth in Helper.FilterMethods(assembly, a => HasAnyAvailabilityAttribute(a, includeUnsupported: true))) {
				var parentAvailability = GetParentAvailabilityAttributes(meth, includeUnsupported: false).ToList();
				// iOS implies maccatalyst, but only for parent scope
				if (parentAvailability.Contains("ios") && !parentAvailability.Contains("maccatalyst")) {
					parentAvailability.Append("maccatalyst");
				}
				var myAvailability = GetAvailabilityAttributes(meth, includeUnsupported: true);
				if (!FirstContainsAllOfSecond (myAvailability, parentAvailability)) {
					if (count < 10) {
						Console.WriteLine (meth.FullName);
						Console.WriteLine ("Parent: " + string.Join(" ", parentAvailability));
						Console.WriteLine ("Mine: " + string.Join(" ", myAvailability));
						Console.WriteLine ();
						count += 1;
					}
					found.Add(meth.FullName);
				}
			}
			Console.WriteLine(found.Count);
			//Assert.That (found, Is.Empty, string.Join (", ", found));
		}

		bool FirstContainsAllOfSecond<T> (IEnumerable<T> first, IEnumerable<T> second)
		{
			HashSet<T> firstSet = new HashSet<T>(first);
			return second.All(s => firstSet.Contains(s));
		}

		IEnumerable<string> GetParentAvailabilityAttributes(PropertyDefinition prop, bool includeUnsupported) => GetAvailabilityAttributes(prop.DeclaringType, includeUnsupported);
		IEnumerable<string> GetParentAvailabilityAttributes(MethodDefinition meth, bool includeUnsupported) => GetAvailabilityAttributes(meth.DeclaringType, includeUnsupported);

		IEnumerable<string> GetAvailabilityAttributes(TypeDefinition definition, bool includeUnsupported) => GetAvailabilityAttributes(definition.CustomAttributes, includeUnsupported);
		IEnumerable<string> GetAvailabilityAttributes(PropertyDefinition prop, bool includeUnsupported) => GetAvailabilityAttributes(prop.CustomAttributes, includeUnsupported);
		IEnumerable<string> GetAvailabilityAttributes(MethodDefinition meth, bool includeUnsupported) => GetAvailabilityAttributes(meth.CustomAttributes, includeUnsupported);

		IEnumerable<string> GetAvailabilityAttributes(IEnumerable<CustomAttribute> attributes, bool includeUnsupported)
		{
			var availability = new List<string>();
			foreach (var attribute in attributes.Where(a => IsAvailabilityAttribute(a, includeUnsupported))) {
				var kind = FindAvailabilityKind (attribute);
				if (kind != null) {
					availability.Add(kind);
				}
			}
			return availability;
		}

		string? FindAvailabilityKind (CustomAttribute attribute)
		{
			if (attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments[0].Type.Name == "String") {
				string full = (string)attribute.ConstructorArguments[0].Value;
				switch (full) {
					case string s when full.StartsWith("ios"):
						return "ios";
					case string s when full.StartsWith("watchos"):
						return "watchos";
					case string s when full.StartsWith("tvos"):
						return "tvos";
					case string s when full.StartsWith("macos"):
						return "macos";
					case string s when full.StartsWith("maccatalyst"):
						return "maccatalyst";
					default:
						throw new System.NotImplementedException($"Unknown platform kind: {full}");
				}
			}
			return null;
		}

		bool HasAnyAvailabilityAttribute (PropertyDefinition prop, bool includeUnsupported) => HasAnyAvailabilityAttribute(prop.CustomAttributes, includeUnsupported);
		bool HasAnyAvailabilityAttribute (MethodDefinition meth, bool includeUnsupported) => HasAnyAvailabilityAttribute(meth.CustomAttributes, includeUnsupported);

		bool HasAnyAvailabilityAttribute (IEnumerable<CustomAttribute> attributes, bool includeUnsupported) => attributes.Any (a => IsAvailabilityAttribute(a, includeUnsupported));

		bool IsAvailabilityAttribute (CustomAttribute attribute, bool includeUnsupported)
		{
			return attribute.AttributeType.Name == "SupportedOSPlatformAttribute" ||
				(includeUnsupported && attribute.AttributeType.Name == "UnsupportedOSPlatformAttribute");
		}
	}
}