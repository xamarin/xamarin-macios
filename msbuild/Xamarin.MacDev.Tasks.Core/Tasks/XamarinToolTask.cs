using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class XamarinToolTask : ToolTask {

		public string SessionId { get; set; }

		[Required]
		public string TargetFrameworkMoniker { get; set; }

		ApplePlatform? platform;
		public ApplePlatform Platform {
			get {
				if (!platform.HasValue)
					platform = PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker);
				return platform.Value;
			}
		}
	}
}
