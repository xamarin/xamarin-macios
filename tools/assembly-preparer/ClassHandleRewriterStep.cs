// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Xamarin.Bundler;
using Xamarin.Linker;

namespace MonoTouch.Tuner {
	public class ClassHandleRewriterStep : ConfigurationAwareStep {
		protected override string Name { get; } = "ClassHandleRewriter";
		protected override int ErrorCode { get; } = 2360;

		protected override void TryEndProcess ()
		{
			if (App.Registrar != RegistrarMode.Static || App.Optimizations.RedirectClassHandles != true)
				return;

			App.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
			App.StaticRegistrar.Rewrite (assembly => Configuration.ModifiedAssemblies.Add (assembly));
		}
	}
}
