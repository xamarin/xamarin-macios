// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// This is a small, dependency-free part of TestRuntime that can be compiled on its own (e.g. into the
// BundledResources assembly, where compiling the full TestRuntime.cs isn't possible because it would
// re-export types that clash with other referenced test assemblies).
partial class TestRuntime {

	// Determine if linkall was enabled by checking if an unused class in this assembly is still here.
	static bool? link_all;
	[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "This property checks whether the trimmer is enabled by checking if a type survived trimming; it's thus trimmer safe in that the any behavioral difference when the trimmer is enabled is exactly what it's looking for.")]
	public static bool IsLinkAll {
		get {
			if (!link_all.HasValue)
				link_all = typeof (TestRuntime).Assembly.GetType (typeof (TestRuntime).FullName + "+LinkerSentinel" + WorkAroundLinkerHeuristics) is null;
			return link_all.Value;
		}
	}
	class LinkerSentinel { }

	// Determine if any assemblies were linked by checking if a few uncommon classes in corlib are still here.
	static bool? link_any;
	[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "This property checks whether the trimmer is enabled by checking if a type survived trimming; it's thus trimmer safe in that the any behavioral difference when the trimmer is enabled is exactly what it's looking for.")]
	public static bool IsLinkAny {
		get {
			if (!link_any.HasValue) {
				var uncommonTypes = new string [] {
					"System.Action`14",
					"System.DBNull",
					"System.Diagnostics.Debugger",
					"System.Func`15",
				};
				link_any = false;
				foreach (var uncommonType in uncommonTypes) {
					link_any = typeof (int).Assembly.GetType (uncommonType + WorkAroundLinkerHeuristics) is null;
					if (link_any == true)
						break;
				}
			}
			return link_any.Value;
		}
	}

	// When using NativeAOT on .NET 11+, ILLink does not run, and assembly post-processing
	// happens after ILC. Attributes we'd otherwise remove (such as [Protocol]) therefore
	// remain at runtime.
	public static bool IsILLinkSkipped {
		get {
#if NATIVEAOT && NET11_0_OR_GREATER
			return true;
#else
			return false;
#endif
		}
	}

	// Returns "" at runtime, but the linker can't constant-fold this, which prevents
	// its dataflow analysis from resolving type names passed to Assembly.GetType.
	[MethodImpl (MethodImplOptions.NoInlining)]
	static string GetEmptyString () => string.Intern ("");
	static string WorkAroundLinkerHeuristics => GetEmptyString ();
}
