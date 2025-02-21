using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public partial class Documentation {
		// Verify that all our publicly visible APIs are documented.
		// This is obviously not true, so we have a rather huge list of known failures.
		// However, this will prevent us from adding more undocumented APIs by accident.
		[Test]
		public void VerifyEveryVisibleMemberIsDocumented ()
		{
			// We join all the APIs from all the platforms, so we can only run this test when all platforms are enabled.
			Configuration.IgnoreIfAnyIgnoredPlatforms ();
			Configuration.IgnoreIfNotXamarinEnabled (); // our tooling to inject docs for Apple APIs lives in maccore, so if we don't do that, we'll get a lot of false positives.

			// Collect everything
			var xmlMembers = new HashSet<AssemblyApi> ();
			var dllMembers = new HashSet<AssemblyApi> ();
			foreach (var info in Helper.NetPlatformAssemblyDefinitions) {
				// Just pick one of the implementation assemblies.
				// We can't just list all the members in the ref assembly, because it doesn't contain any private members, and some private members are documented.
				var implementationAssembly = Helper.NetPlatformImplementationAssemblyDefinitions.Where (v => Path.GetFileName (v.Path) == Path.GetFileName (info.Path)).First ();

				var xml = GetDocumentedMembers (Path.ChangeExtension (info.Path, ".xml"));
				xmlMembers.UnionWith (xml);
				var dll = GetAssemblyMembers (implementationAssembly.Assembly);
				dllMembers.UnionWith (dll);
			}

			// Propagate publicness
			foreach (var dll in dllMembers) {
				if (xmlMembers.TryGetValue (dll, out var xml)) {
					xml.PubliclyVisible = dll.PubliclyVisible;
				}
			}

			var documentedButNotPresent = xmlMembers.Except (dllMembers).ToList ();
			Assert.Multiple (() => {
				foreach (var doc in documentedButNotPresent)
					Assert.Fail ($"{doc.DocId}: Documented API not found in the platform assembly. This probably indicates that the code to compute the doc name for a given member is incorrect.");
			});

			var shouldHaveBeenDocumented = dllMembers
											.Except (xmlMembers)
											.Where (v => v.PubliclyVisible == true && v.DocsRequired)
											.Select (v => v.DocId)
											.OrderBy (v => v)
											.ToList ();
			var knownfailuresFilename = $"Documentation.KnownFailures.txt";
			var knownfailuresPath = Path.Combine (Configuration.SourceRoot, "tests", "cecil-tests", knownfailuresFilename);
			var knownfailures = File.Exists (knownfailuresPath) ? File.ReadAllLines (knownfailuresPath) : Array.Empty<string> ();

			var unknownFailures = shouldHaveBeenDocumented.Except (knownfailures).ToList ();
			var fixedFailures = knownfailures.Except (shouldHaveBeenDocumented).ToList ();

			if (unknownFailures.Any () || fixedFailures.Any ()) {
				if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("WRITE_KNOWN_FAILURES"))) {
					File.WriteAllLines (knownfailuresPath, shouldHaveBeenDocumented);
					Assert.Fail ($"Found {unknownFailures.Count} undocumented APIs (not known failures) and {fixedFailures.Count} APIs that were marked as known failures but are now documented. The known failures have been updated, so please commit the results. Re-running the test should now succeed.");
				} else {
					if (unknownFailures.Any ()) {
						Console.WriteLine ($"Undocumented APIs that aren't known failures (found {unknownFailures.Count}, showing at most 10):");
						foreach (var failure in unknownFailures.Take (10))
							Console.WriteLine ($"    {failure}");
						if (unknownFailures.Count > 10)
							Console.WriteLine ($"    ... and {unknownFailures.Count () - 10} more");
					}
					if (fixedFailures.Any ()) {
						Console.WriteLine ($"Documented APIs that are known failures (found {fixedFailures.Count}, showing at most 10):");
						foreach (var failure in fixedFailures.Take (10))
							Console.WriteLine ($"    {failure}");
						if (fixedFailures.Count > 10)
							Console.WriteLine ($"    ... and {fixedFailures.Count () - 10} more");
					}
					Assert.Fail ($"Found {unknownFailures.Count} undocumented APIs (not known failures) and {fixedFailures.Count} APIs that were marked as known failures but are now documented. If this is expected, set the WRITE_KNOWN_FAILURES=1 environment variable, run the test again, and commit the modified known failures file.");
				}
			}
		}

		static HashSet<AssemblyApi> GetDocumentedMembers (string xml)
		{
			var rv = new HashSet<AssemblyApi> ();
			var doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (xml);
			foreach (XmlNode node in doc.SelectNodes ("/doc/members/member")!) {
				rv.Add (new AssemblyApi (node.Attributes! ["name"]!.Value!, null, false));
			}
			return rv;
		}

		class AssemblyApi {
			public string DocId;
			public bool? PubliclyVisible;
			public bool DocsRequired;

			public AssemblyApi (string docId, bool? visible, bool docsRequired)
			{
				DocId = docId;
				PubliclyVisible = visible;
				DocsRequired = docsRequired;
			}

			public override int GetHashCode ()
			{
				return DocId.GetHashCode ();
			}

			public override bool Equals (object? other)
			{
				return other is AssemblyApi aa && aa.DocId == DocId;
			}
		}

		static HashSet<AssemblyApi> GetAssemblyMembers (AssemblyDefinition asm)
		{
			var rv = new HashSet<AssemblyApi> ();

			foreach (var member in asm.EnumerateMembers ()) {
				string name;

				if (member is MethodDefinition md) {
					// It's not possible to add xml documentation to members of delegates, so don't verify those
					if (IsDelegateType (md.DeclaringType))
						continue;
					// It's not possible to add xml documentation to getters/setters (it's added to the property itself), so don't verify those.
					if (md.IsPropertyAccessor ())
						continue;
					name = "M:" + GetDocId (md);
				} else if (member is PropertyDefinition pd) {
					name = "P:" + GetDocId (pd);
				} else if (member is FieldDefinition fd) {
					// It's not possible to add xml documentation to the 'value__' field for enums, so don't verify those
					if (fd.Name == "value__" && fd.DeclaringType.BaseType.Is ("System", "Enum"))
						continue;
					name = "F:" + GetDocId (fd.DeclaringType) + "." + fd.Name;
				} else if (member is EventDefinition ed) {
					name = "E:" + GetDocId (ed);
				} else if (member is TypeDefinition td) {
					name = "T:" + GetDocId (td);
				} else {
					throw new NotImplementedException ($"Unknown member type: {member.GetType ()}");
				}
				var docsRequired = true;
				if (member is ICustomAttributeProvider provider) {
					if (provider.HasEditorBrowseableNeverAttribute () || provider.IsObsolete ())
						docsRequired = false;
				}
				rv.Add (new AssemblyApi (name, member.IsPubliclyVisible (), docsRequired));
			}

			return rv;
		}

		static bool IsDelegateType (TypeDefinition? type)
		{
			while (type is not null) {
				if (type.Is ("System", "Delegate"))
					return true;

				type = type.BaseType is not null ? type.BaseType.Resolve () : null;
			}

			return false;
		}

		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#id-strings
		// See src/bgen/DocumentationManager.cs for an implementation that works with System.Reflection.
		// There's already an implementation in Roslyn, but that's a rather heavy dependency,
		// so we're implementing this in our own code instead.

		static string GetDocId (MethodDefinition md)
		{
			var methodName = md.Name.Replace ('.', '#');
			var name = GetDocId (md.DeclaringType) + "." + methodName;
			if (md.HasGenericParameters)
				name += $"``{md.GenericParameters.Count}";
			if (md.HasParameters) {
				name += "(" + string.Join (",", md.Parameters.Select (p => GetDocId (p.ParameterType))) + ")";
			}

			if (md.Name == "op_Explicit" || md.Name == "op_Implicit") {
				name += "~" + GetDocId (md.ReturnType);
			}

			return name;
		}

		static string GetDocId (EventDefinition ed) => GetDocId (ed.DeclaringType) + "." + ed.Name;

		static string GetDocId (PropertyDefinition pd)
		{
			var propertyName = pd.Name.Replace ('.', '#');
			var name = GetDocId (pd.DeclaringType) + "." + propertyName;
			if (pd.HasParameters) {
				name += "(" + string.Join (",", pd.Parameters.Select (p => GetDocId (p.ParameterType))) + ")";
			}
			return name;
		}

		static string GetDocId (TypeReference tr)
		{
			string name = "";
			if (tr.IsNested) {
				var decl = tr.DeclaringType;
				while (decl.IsNested) {
					name = decl.Name + "." + name;
					decl = decl.DeclaringType;
				}
				name = decl.Namespace + "." + decl.Name + "." + name;
			} else {
				name = tr.Namespace + ".";
			}

			if (tr is GenericInstanceType git) {
				name += git.Name [0..(git.Name.IndexOf ('`'))];
				name += "{" + string.Join (",", git.GenericArguments.Select (v => GetDocId (v))) + "}";
			} else if (tr is TypeDefinition td && td.HasGenericParameters) {
				name += tr.Name;
			} else if (tr is ByReferenceType brt) {
				name += brt.ElementType.Name + "@";
			} else if (tr is GenericParameter gp) {
				if (gp.DeclaringMethod is not null) {
					name = $"``{gp.Position}";
				} else {
					name = $"`{gp.Position}";
				}
			} else if (tr is ArrayType at) {
				name = GetDocId (at.ElementType);
				if (at.Rank == 1) {
					name += "[]";
				} else {
					name += "[";
					for (var d = 0; d < at.Dimensions.Count; d++) {
						if (d > 0)
							name += ",";
						var dim = at.Dimensions [d];
						if (dim.LowerBound.HasValue)
							name += dim.LowerBound.Value;
						name += ":";
						if (dim.UpperBound.HasValue)
							name += dim.UpperBound.Value;
					}
					name += "]";
				}
			} else {
				name += tr.Name;
			}
			return name;
		}
	}
}
