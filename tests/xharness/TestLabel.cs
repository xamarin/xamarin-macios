using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable

namespace Xharness {
	[AttributeUsage (AttributeTargets.Field)]
	public class LabelAttribute : Attribute {
		public string Label { get; }

		public LabelAttribute (string label)
		{
			Label = label;
		}
	}

	[Flags]
	public enum TestLabel : Int64 {
		[Label ("none")]
		None = 0,
		// 1 << 1 is unused
		[Label ("bgen")]
		Bgen = 1 << 2,
		[Label ("binding")]
		Binding = 1 << 3,
		[Label ("bindings-framework")]
		BindingFramework = 1 << 4,
		[Label ("bindings-xcframework")]
		BindingsXcframework = 1 << 5,
		[Label ("cecil")]
		Cecil = 1 << 6,
		[Label ("docs")]
		Docs = 1 << 7,
		[Label ("dotnettests")]
		DotnetTest = 1 << 8,
		[Label ("fsharp")]
		Fsharp = 1 << 9,
		[Label ("framework")]
		Framework = 1 << 10,
		[Label ("generator")]
		Generator = 1 << 11,
		[Label ("interdependent-binding-projects")]
		InterdependentBindingProjects = 1 << 12,
		// 1 << 13 is unused
		[Label ("introspection")]
		Introspection = 1 << 14,
		[Label ("linker")]
		Linker = 1 << 15,
		[Label ("library-projects")]
		LibraryProjects = 1 << 16,
		// 1 << 17 is unused
		// 1 << 18 is unused
		// 1 << 19 is unused
		[Label ("monotouch")]
		Monotouch = 1 << 20,
		[Label ("msbuild")]
		Msbuild = 1 << 21,
		// 1 << 22 is unused
		[Label ("sampletester")]
		SampleTester = 1 << 23,
		[Label ("system-permission")]
		SystemPermission = 1 << 24,
		//  1 << 25 is unused
		[Label ("xcframework")]
		Xcframework = 1 << 26,
		[Label ("xtro")]
		Xtro = 1 << 27,
		[Label ("packaged-macos")]
		PackagedMacOS = 1 << 28,
		[Label ("windows")]
		Windows = 1 << 29,
		[Label ("all")]
		All = Int64.MaxValue,
	}

	[Flags]
	public enum PlatformLabel : uint {
		[Label ("none")]
		None = 0,
		[Label ("device")]
		Device = 1 << 1,
		[Label ("ios")]
		iOS = 1 << 2,
		[Label ("ios-extensions")]
		iOSExtension = 1 << 3,
		[Label ("ios-simulator")]
		iOSSimulator = 1 << 4,
		[Label ("mac")]
		Mac = 1 << 7,
		[Label ("maccatalyst")]
		MacCatalyst = 1 << 8,
		[Label ("old-simulator")]
		OldiOSSimulator = 1 << 9,
		[Label ("tvos")]
		tvOS = 1 << 10,
		// 1 << 11 is unused
		// 1 << 12 is unused
		// 1 << 13 is unused
		[Label ("all")]
		All = 0xFFFFFFFF,
	}

	static class TestLabelExtensions {
		static string GetLabel<T> (this T self) where T : Enum
		{
			var name = Enum.GetName (typeof (T), self)!;
			var attr = typeof (T).GetField (name)!.GetCustomAttribute<LabelAttribute> ();
			return attr!.Label;
		}

		public static bool TryGetLabel<T> (this string self, out T label) where T : struct, Enum
		{
#if NET
			foreach (var obj in Enum.GetValues<T> ()) {
#else
			foreach (var obj in Enum.GetValues (typeof (T))) {
#endif
				if (obj is T value && value.GetLabel () == self) {
					label = value;
					return true;
				}
			}

			label = default;
			return false;
		}
	}
}
