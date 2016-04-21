//
// DetectSigningIdentityTask.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc.

using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class DetectSigningIdentity : DetectSigningIdentityTaskBase
	{
		static readonly string[] appStoreDistributionPrefixes = { "3rd Party Mac Developer Application" };
		static readonly string[] directDistributionPrefixes = { "Developer ID Application" };
		static readonly string[] developmentPrefixes = { "Mac Developer" };

		protected override string[] DevelopmentPrefixes { get { return developmentPrefixes; } }
		protected override string[] DirectDistributionPrefixes { get { return directDistributionPrefixes; } }
		protected override string[] AppStoreDistributionPrefixes { get { return appStoreDistributionPrefixes; } }
		protected override string DeveloperRoot { get { return MacOSXSdks.Native.DeveloperRoot; } }
		protected override PlatformFramework Framework { get { return PlatformFramework.MacOS; } }
		protected override string PlatformName { get { return "OS X"; } }
		protected override string ApplicationIdentifierKey { get { return "com.apple.application-identifier"; } }
	}
}
