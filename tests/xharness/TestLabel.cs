using System;
using System.Diagnostics;
using System.Linq;
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
	public enum TestLabel {
		//{ "all", false },
		[Label ("all")]
		All,
		//{ "bcl", false },
		[Label ("bcl")]
		Bcl,
		//{ "mac", true },
		[Label ("mac")]
		Mac,
		//{ "ios", true },
		[Label ("ios")]
		iOS,
		//{ "ios-32", false },
		[Label ("ios-32")]
		iOS32,
		//{ "ios-64", true },
		[Label ("ios-64")]
		iOs64,
		//{ "ios-extensions", false },
		[Label ("ios-extensions")]
		iOSExtension,
		//{ "ios-simulator", true },
		[Label ("ios-simulator")]
		iOSSimulator,
		//{ "old-simulator", false },
		[Label ("old-simulator")]
		OldiOSSimulator,
		//{ "device", false },
		[Label ("device")]
		Device,
		//{ "xtro", false },
		[Label ("xtro")]
		Xtro,
		//{ "cecil", false },
		[Label ("cecil")]
		Cecil,
		//{ "docs", false },
		[Label ("docs")]
		Docs,
		//{ "bcl-xunit", false },
		[Label ("bcl-xunit")]
		BclXUnit,
		//{ "bcl-nunit", false },
		[Label ("bcl-nunit")]
		BclNUnit,
		//{ "mscorlib", false },
		[Label ("mscorlib")]
		Mscorlib,
		//{ "non-monotouch", true }, 
		[Label ("non-monotouch")]
		NonMonotouch,
		//{ "monotouch", true }, 
		[Label ("monotouch")]
		Monotouch,
		//{ "dotnet", false },
		[Label ("dotnet")]
		Dotnet,
		//{ "maccatalyst", true },
		[Label ("maccatalyst")]
		MacCatalyst,
		//{ "tvos", true },
		[Label ("tvos")]
		tvOS,
		//{ "watchos", true },
		[Label ("watchos")]
		watchOS,
		//{ "mmp", false },
		[Label ("mmp")]
		Mmp,
		//{ "msbuild", true },
		[Label ("msbuild")]
		Msbuild,
		//{ "mtouch", false },
		[Label ("mtouch")]
		Mtouch,
		//{ "btouch", false },
		[Label ("btouch")]
		Btouch,
		//{ "mac-binding-project", false },
		[Label ("mac-binding-project")]
		MacBindingProject,
		//{ "system-permission", false },
		[Label ("system-permission")]
		SystemPermission,
	}

	static class TestLabelExtensions {
		public static string GetLabel (this TestLabel self)
		{
			var enumType = typeof(TestLabel);
			var name = Enum.GetName(typeof(TestLabel), self);
			var attr = enumType.GetField(name).GetCustomAttributes(false)
				.OfType<LabelAttribute>().SingleOrDefault();
			return attr.Label;
		}

		public static TestLabel GetLabel (this string self)
		{
			foreach (var l in Enum.GetValues (typeof(TestLabel))) {
				var value = (TestLabel) l;
				if (value.GetLabel () == self) {
					return value;
				}
			}

			throw new InvalidOperationException ($"Unknown label '{self}'");
		}
	}
}
