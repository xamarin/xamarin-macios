using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Unix.Native;

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
	public enum TestLabel : ulong {
		[Label ("none")]
		None = 0,
		[Label ("bcl")]
		Bcl = 1 << 1,
		[Label ("bcl-nunit")]
		BclNUnit = 1 << 2,
		[Label ("bcl-xunit")]
		BclXUnit = 1 << 3,
		[Label ("btouch")]
		Btouch = 1 << 4,
		[Label ("cecil")]
		Cecil = 1 << 5,
		[Label ("device")]
		Device = 1 << 6,
		[Label ("docs")]
		Docs = 1 << 7,
		[Label ("dotnet")]
		Dotnet = 1 << 8,
		[Label ("ios")]
		iOS = 1 << 9,
		[Label ("ios-extensions")]
		iOSExtension = 1 << 10,
		[Label ("ios-simulator")]
		iOSSimulator = 1 << 11,
		[Label ("ios-32")]
		iOS32 = 1 << 12,
		[Label ("ios-64")]
		iOS64 = 1 << 13,
		[Label ("mac")]
		Mac = 1 << 14,
		[Label ("mac-binding-project")]
		MacBindingProject = 1 << 15,
		[Label ("maccatalyst")]
		MacCatalyst = 1 << 16,
		[Label ("mmp")]
		Mmp = 1 << 17,
		[Label ("mscorlib")]
		Mscorlib = 1 << 18,
		[Label ("monotouch")]
		Monotouch = 1 << 19,
		[Label ("msbuild")]
		Msbuild = 1 << 20,
		[Label ("mtouch")]
		Mtouch = 1 << 21,
		[Label ("non-monotouch")]
		NonMonotouch = 1 << 22,
		[Label ("old-simulator")]
		OldiOSSimulator = 1 << 23,
		[Label ("system-permission")]
		SystemPermission = 1 << 24,
		[Label ("tvos")]
		tvOS = 1 << 15,
		[Label ("watchos")]
		watchOS = 1 << 26,
		[Label ("xtro")]
		Xtro = 1 << 27,
		[Label ("all")]
		All =  0xFFFFFFFF,
	}

	static class TestLabelExtensions {
		public static string GetLabel (this TestLabel self)
		{
			var enumType = typeof(TestLabel);
			var name = Enum.GetName (typeof (TestLabel), self);
			var attr = enumType.GetField (name).GetCustomAttribute<LabelAttribute> ();
			return attr.Label;
		}

		public static TestLabel GetLabel (this string self)
		{
			foreach (var obj in Enum.GetValues (typeof(TestLabel))) {
				if (obj is TestLabel value && value.GetLabel () == self) {
					return value;
				}
			}

			throw new InvalidOperationException ($"Unknown label '{self}'");
		}
	}
}
