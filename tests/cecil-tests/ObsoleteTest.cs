using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ObsoleteTest {
		// This test verifies that we don't have any obsolete API in .NET that we don't expect to be there
		// in particular that we don't start out with obsolete APIs from the very beginning (such API should have been removed).
		// Any obsoleted API after the first stable .NET release should likely be skipped (until XAMCORE_5_0)
		//
		// If you have obsoleted a member and you're here wondering
		// what you should do, you should add
		// [EditorBrowsable (EditorBrowsableState.Never)]
		// to the member in addition to the Obsolete. This will
		// hide the member from intellisense but still allow it
		// to compile (with errors/warnings).
		//
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))] // call this method with every .net6 library
		public void GetAllObsoletedThings (AssemblyInfo info)
		{
			var assembly = info.Assembly;

			// Make a list of Obsolete things
			var found = new HashSet<string> ();

			foreach (var prop in assembly.EnumerateAttributeProviders (a => FilterMember (a))) {
				Console.WriteLine ($"{prop.RenderLocation ()} {prop.AsFullName ()}: add '[EditorBrowsable (EditorBrowsableState.Never)]' for newly obsoleted API to pass this test.");
				found.Add (prop.AsFullName ());
			}

			Assert.That (found, Is.Empty, "Obsolete API: add '[EditorBrowsable (EditorBrowsableState.Never)]' for newly obsoleted API to pass this test.");
		}

		bool FilterMember (ICustomAttributeProvider provider)
		{
			// If an API isn't obsolete, it's not under scrutiny from this test.
			if (!provider.IsObsolete ())
				return false;

			// If the API has an UnsupportedOSPlatform attribute with a version, it means the API is available
			// on earlier OS versions, which means we can't remove it.
			if (HasVersionedUnsupportedOSPlatformAttribute (provider))
				return false;

#if !XAMCORE_5_0
			// If we've hidden an API from the IDE, assume we've decided to keep the API for binary compatibility
			// At least until the next time we can do breaking changes.
			if (provider.HasEditorBrowseableNeverAttribute ())
				return false;
#endif

			// I'm bad!
			return true;
		}

		bool HasVersionedUnsupportedOSPlatformAttribute (ICustomAttributeProvider provider)
		{
			if (provider?.HasCustomAttributes != true)
				return false;

			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.Name != "UnsupportedOSPlatformAttribute")
					continue;
				var platform = (string) attr.ConstructorArguments [0].Value;
				// is this a platform string with a version?
				foreach (var ch in platform) {
					if (ch >= '0' && ch <= '9')
						return true;
				}
			}

			// no UnsupportedOSPlatform attribute with a version here
			return false;
		}

		public static bool HasEditorBrowseableNeverAttribute (ICustomAttributeProvider provider)
		{
			if (provider?.HasCustomAttributes != true)
				return false;

			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.Name != "EditorBrowsableAttribute")
					continue;
				var state = (EditorBrowsableState) attr.ConstructorArguments [0].Value;
				if (state == EditorBrowsableState.Never)
					return true;
			}

			return false;
		}
	}
}
