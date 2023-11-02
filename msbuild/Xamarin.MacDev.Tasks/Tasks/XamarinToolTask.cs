using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	// This is the same as XamarinTask, except that it subclasses ToolTask instead.
	public abstract class XamarinToolTask : ToolTask {

		public string SessionId { get; set; } = string.Empty;

		public string TargetFrameworkMoniker { get; set; } = string.Empty;

		void VerifyTargetFrameworkMoniker ()
		{
			if (!string.IsNullOrEmpty (TargetFrameworkMoniker))
				return;
			Log.LogError ($"The task {GetType ().Name} requires TargetFrameworkMoniker to be set.");
		}

		public string Product {
			get {
				if (IsDotNet)
					return "Microsoft." + PlatformName;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "Xamarin.iOS";
				case ApplePlatform.MacOSX:
					return "Xamarin.Mac";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		ApplePlatform? platform;
		public ApplePlatform Platform {
			get {
				if (!platform.HasValue)
					platform = PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker);
				return platform.Value;
			}
		}

		TargetFramework? target_framework;
		public TargetFramework TargetFramework {
			get {
				if (!target_framework.HasValue) {
					VerifyTargetFrameworkMoniker ();
					target_framework = TargetFramework.Parse (TargetFrameworkMoniker);
				}
				return target_framework.Value;
			}
		}

		public bool IsDotNet {
			get { return TargetFramework.IsDotNet; }
		}

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.WatchOS:
					return "watchOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		public bool ShouldExecuteRemotely () => this.ShouldExecuteRemotely (SessionId);

		protected internal static IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (params string [] directories)
		{
			return XamarinTask.CreateItemsForAllFilesRecursively ((IEnumerable<string>?) directories);
		}
	}
}
