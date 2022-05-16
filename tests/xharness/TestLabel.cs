using System;
using System.Reflection;

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
		[Label ("bcl")]
		Bcl = 1 << 1,
		[Label("bgen")]
		Bgen = 1 << 2,
		[Label ("binding")]
		Binding = 1 << 3,
		[Label ("bindings-framework")]
		BindingFramework = 1 << 4,
		[Label("bindings-xcframework")]
		BindingsXcframework = 1 << 5,
		[Label ("btouch")]
		Btouch = 1 << 6,
		[Label ("cecil")]
		Cecil = 1 << 7,
		[Label ("docs")]
		Docs = 1 << 8,
		[Label ("dotnet")]
		Dotnet = 1 << 9,
		[Label ("fsharp")]
		Fsharp = 1 << 10,
		[Label("framework")]
		Framework = 1 << 11,
		[Label ("geneator")]
		Generator = 1 << 12,
		[Label ("interdependent-binding-projects")]
		InterdependentBindingProjects = 1 << 13,
		[Label("install-source")]
		InstallSource = 1 << 14,
		[Label ("introspection")]
		Introspection = 1 << 15,
		[Label ("linker")]
		Linker = 1 << 16,
		[Label ("library-projects")]
		LibraryProjects = 1 << 17,
		[Label ("mac-binding-project")]
		MacBindingProject = 1 << 18,
		[Label ("mmp")]
		Mmp = 1 << 19,
		[Label ("mononative")]
		Mononative = 1 << 20,
		[Label ("monotouch")]
		Monotouch = 1 << 21,
		[Label ("msbuild")]
		Msbuild = 1 << 22,
		[Label ("mtouch")]
		Mtouch = 1 << 23,
		[Label ("sampletester")]
		SampleTester = 1 << 24,
		[Label ("system-permission")]
		SystemPermission = 1 << 25,
		[Label ("xammac")]
		Xammac = 1 << 26,
		[Label ("xcframework")]
		Xcframework = 1 << 27,
		[Label ("xtro")]
		Xtro = 1 << 28,
		[Label ("all")]
		All =  Int64.MaxValue, 
	}

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
		[Label ("ios-32")]
		iOS32 = 1 << 5,
		[Label ("ios-64")]
		iOS64 = 1 << 6,
		[Label ("mac")]
		Mac = 1 << 7,
		[Label ("maccatalyst")]
		MacCatalyst = 1 << 8,
		[Label ("old-simulator")]
		OldiOSSimulator = 1 << 9,
		[Label ("tvos")]
		tvOS = 1 << 10,
		[Label ("watchos")]
		watchOS = 1 << 11,
		[Label ("all")]
		All =  0xFFFFFFFF, 
	}

	static class TestLabelExtensions {
		static string GetLabel<T> (this T self) where T : Enum
		{
			var enumType = self.GetType ();
			var name = Enum.GetName(typeof(T), self);
			var attr = enumType.GetField (name).GetCustomAttribute<LabelAttribute> ();
			return attr.Label;
		}

		public static bool TryGetLabel<T> (this string self, out T? label) where T : Enum
		{
			foreach (var obj in Enum.GetValues (typeof(T))) {
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
