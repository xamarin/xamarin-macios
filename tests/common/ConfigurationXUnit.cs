using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Utils;
using Xunit.Sdk;

namespace Xamarin.Tests {

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class PlatformInlineDataAttribute : DataAttribute {
		readonly object [] dataValues;
		public PlatformInlineDataAttribute (ApplePlatform platform, params object [] parameters)
		{
			// data values are the join of the platform and all other values passed to the attr
			dataValues = parameters.Prepend (platform).ToArray ();
			// based on the passed platform and the configuration, decide if we skip the test
			switch (platform) {
			case ApplePlatform.iOS:
				if (!Configuration.include_ios)
					Skip = "iOS is not included in this build";
				break;
			case ApplePlatform.TVOS:
				if (!Configuration.include_tvos)
					Skip = "tvOS is not included in this build";
				break;
			case ApplePlatform.MacOSX:
				if (!Configuration.include_mac)
					Skip = "macOS is not included in this build";
				break;
			case ApplePlatform.MacCatalyst:
				if (!Configuration.include_maccatalyst)
					Skip = "Mac Catalyst is not included in this build";
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		public object [] DataValues {
			get { return dataValues; }
		}

		public override IEnumerable<object []> GetData (MethodInfo testMethod)
		{
			yield return dataValues;
		}
	}

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class AllSupportedPlatformsAttribute : DataAttribute {

		readonly object [] dataValues;

		public AllSupportedPlatformsAttribute (params object [] parameters)
		{
			dataValues = parameters;
		}

		public override IEnumerable<object []> GetData (MethodInfo testMethod)
		{
			return Configuration.
				GetIncludedPlatforms (true).
				Select (v => dataValues.Prepend (platform).ToArray ());
		}
	}

	public partial class Configuration {
		static string TestAssemblyDirectory {
			get {
				return Assembly.GetExecutingAssembly ().Location;
			}
		}

		public static bool IsEnabled (ApplePlatform platform)
			=> platform switch {
				ApplePlatform.iOS => include_ios,
				ApplePlatform.TVOS => include_tvos,
				ApplePlatform.MacCatalyst => include_maccatalyst,
				ApplePlatform.MacOSX => include_mac,
				_ => false
			};
	}
}
