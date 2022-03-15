using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ObsoleteTest {

#if NET
		[Ignore ("To be fixed after the move to an outside bot: https://github.com/xamarin/maccore/issues/2547.")]
#endif
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblies))] // call this method with every .net6 library
		public void GetAllObsoletedThings (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath, readSymbols: true)!;
			Assert.That (assembly, Is.Not.Null, "Must find the assembly");

			// Make a list of Obsolete things
			var found = new HashSet<string> ();

			foreach (var prop in Helper.FilterProperties (assembly, a => FilterMember (a))) {
				if (Skip (prop))
					continue;
				Console.WriteLine ($"{GetLocation (prop.GetMethod ?? prop.SetMethod)} {prop.FullName}");
				found.Add (prop.FullName);
			}

			foreach (var meth in Helper.FilterMethods (assembly, a => FilterMember (a))) {
				if (Skip (meth))
					continue;
				Console.WriteLine ($"{GetLocation (meth)} {meth.FullName}");
				found.Add (meth.FullName);
			}

			foreach (var type in Helper.FilterTypes (assembly, a => FilterMember (a))) {
				if (Skip (type))
					continue;
				Console.WriteLine ($"{GetLocation (type.Methods.FirstOrDefault ())} {type.FullName}");
				found.Add (type.FullName);
			}

			// TODO: Events?
			Assert.That (found, Is.Empty, "Obsolete API");
		}

		bool FilterMember (ICustomAttributeProvider provider)
		{
			// If an API isn't obsolete, it's not under scrutiny from this test.
			if (!HasObsoleteAttribute (provider))
				return false;

			// If the API has an UnsupportedOSPlatform attribute with a version, it means the API is available
			// on earlier OS versions, which means we can't remove it.
			if (HasVersionedUnsupportedOSPlatformAttribute (provider))
				return false;

#if !XAMCORE_5_0
			// If we've hidden an API from the IDE, assume we've decided to keep the API for binary compatibility
			// At least until the next time we can do breaking changes.
			if (HasEditorBrowseableNeverAttribute (provider))
				return false;
#endif

			// I'm bad!
			return true;
		}

		bool HasObsoleteAttribute (ICustomAttributeProvider provider) => HasObsoleteAttribute (provider.CustomAttributes);

		bool HasObsoleteAttribute (IEnumerable<CustomAttribute> attributes) => attributes.Any (a => IsObsoleteAttribute (a));

		bool IsObsoleteAttribute (CustomAttribute attribute) => attribute.AttributeType.Name == "Obsolete" || (attribute.AttributeType.Name == "ObsoleteAttribute");

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

		bool HasEditorBrowseableNeverAttribute (ICustomAttributeProvider provider)
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

		bool Skip (MemberReference member)
		{
			var ns = member.FullName.Split ('.') [0];

			switch (ns) {
			default:
				return false;
			}
		}

		static string GetLocation (MethodDefinition method)
		{
			if (method is null)
				return "<no location> ";

			if (method.DebugInformation.HasSequencePoints) {
				var seq = method.DebugInformation.SequencePoints [0];
				return seq.Document.Url + ":" + seq.StartLine + " ";
			}
			return string.Empty;
		}
	}
}
