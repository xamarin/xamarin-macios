using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	// [TestFixture]
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
		// [TestCaseSource (typeof (Helper), "NetPlatformAssemblies")]
		public void ChildElementsListAvailabilityForAllPlatformsOnParent (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

#if DEBUG
			Console.WriteLine(assemblyPath);
#endif

			// Make a list of Availability on parent.
			// Each child must have an attribute for each availability (Either NotSupported or Supported)
			HashSet<string> found = new HashSet<string> ();
			foreach (var prop in Helper.FilterProperties(assembly, a => HasAnyAvailabilityAttribute(a, includeUnsupported: true))) {
				Process (prop, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in Helper.FilterMethods(assembly, a => HasAnyAvailabilityAttribute(a, includeUnsupported: true))) {
				Process (meth, meth.FullName, meth.DeclaringType, found);
			}
#if DEBUG				
			Assert.That (found.Count, Is.Zero);
#else
			Assert.That (found.Count, Is.Zero, string.Join (", ", found));
#endif
		}

		void Process (object item, string fullName, TypeDefinition parent, HashSet<string> found)
		{
			// XXX - For now skip generated code until associated generator.cs changes are in
			if (Ignore (fullName) || HasCodegenAttribute(item)) {
				return;
			}
// #if DEBUG
// 			const string Filter = "AppKit";
// 			if (!fullName.Contains (" " + Filter)) {
// 				return;
// 			}
// #endif
		
			var parentAvailability = GetAvailabilityAttributes(parent, includeUnsupported: false).ToList();
// 			// This is true in theory, but our code should be explicit and list every platform individually
// 			// This can be re-enabled if that decision is reverted.

// 			// iOS implies maccatalyst, but only for parent scope
// 			if (parentAvailability.Contains("ios") && !parentAvailability.Contains("maccatalyst")) {
// 				parentAvailability.Append("maccatalyst");
// 			}

			var myAvailability = GetAvailabilityAttributes(item, includeUnsupported: true);
			if (!FirstContainsAllOfSecond (myAvailability, parentAvailability)) {
				DebugPrint (fullName, parentAvailability, myAvailability);
				found.Add(fullName);
			}
		}
		
		[Conditional("DEBUG")]
		void DebugPrint (string fullName, IEnumerable<string> parentAvailability, IEnumerable<string> myAvailability)
		{
			Console.WriteLine (fullName);
			Console.WriteLine ("Parent: " + string.Join(" ", parentAvailability));
			Console.WriteLine ("Mine: " + string.Join(" ", myAvailability));
			Console.WriteLine ();
		}

		bool Ignore (string fullName)
		{
			switch (fullName) {
				default:
					return false;
			}
		}

		bool FirstContainsAllOfSecond<T> (IEnumerable<T> first, IEnumerable<T> second)
		{
			var firstSet = new HashSet<T>(first);
			return second.All(s => firstSet.Contains(s));
		}

		IEnumerable<string> GetAvailabilityAttributes(object o, bool includeUnsupported)
		{
			switch (o) {
				case TypeDefinition definition:
					return GetAvailabilityAttributes (definition, includeUnsupported);
				case PropertyDefinition prop:
					return GetAvailabilityAttributes (prop, includeUnsupported);
				case MethodDefinition meth:
					return GetAvailabilityAttributes (meth, includeUnsupported);
				default:
					throw new NotImplementedException ();
			}
		}

		IEnumerable<string> GetAvailabilityAttributes(TypeDefinition definition, bool includeUnsupported) => GetAvailabilityAttributes(definition.CustomAttributes, includeUnsupported);
		IEnumerable<string> GetAvailabilityAttributes(PropertyDefinition prop, bool includeUnsupported) => GetAvailabilityAttributes(prop.CustomAttributes, includeUnsupported);
		IEnumerable<string> GetAvailabilityAttributes(MethodDefinition meth, bool includeUnsupported) => GetAvailabilityAttributes(meth.CustomAttributes, includeUnsupported);

		IEnumerable<string> GetAvailabilityAttributes(IEnumerable<CustomAttribute> attributes, bool includeUnsupported)
		{
			var availability = new List<string>();
			foreach (var attribute in attributes.Where(a => IsAvailabilityAttribute(a, includeUnsupported))) {
				var kind = FindAvailabilityKind (attribute);
				if (kind is not null) {
					availability.Add(kind);
				}
			}
			return availability;
		}

		bool HasCodegenAttribute(object o)
		{
			switch (o) {
				case PropertyDefinition prop:
					return HasCodegenAttribute (prop);
				case MethodDefinition meth:
					return HasCodegenAttribute (meth);
				default:
					throw new NotImplementedException ();
			}
		}

		bool HasCodegenAttribute(PropertyDefinition prop) => HasCodegenAttribute(prop.CustomAttributes);
		bool HasCodegenAttribute(MethodDefinition meth) => HasCodegenAttribute(meth.CustomAttributes);
		bool HasCodegenAttribute(IEnumerable<CustomAttribute> attributes) => attributes.Any(a => a.AttributeType.Name == "BindingImplAttribute");

		string? FindAvailabilityKind (CustomAttribute attribute)
		{
			if (attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments[0].Type.Name == "String") {
				string full = (string)attribute.ConstructorArguments[0].Value;
				switch (full) {
					case string s when full.StartsWith("ios"):
						return "ios";
					case string s when full.StartsWith("tvos"):
						return "tvos";
					case string s when full.StartsWith("macos"):
						return "macos";
					case string s when full.StartsWith("maccatalyst"):
						return "maccatalyst";
					case string s when full.StartsWith("watchos"):
						return null; // WatchOS is ignored for comparision
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