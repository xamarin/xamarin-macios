using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Mono.Linker;
using Mono.Linker.Steps;

using MonoTouch.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;

#nullable enable

namespace Xamarin {

	public class SetupStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Setup";
		protected override int ErrorCode { get; } = 2300;

		protected override void TryProcess ()
		{
			Configuration.Write ();
			ErrorHelper.Platform = Configuration.Platform;
			Directory.CreateDirectory (Configuration.ItemsDirectory);
			Directory.CreateDirectory (Configuration.CacheDirectory);
		}
	}
}
