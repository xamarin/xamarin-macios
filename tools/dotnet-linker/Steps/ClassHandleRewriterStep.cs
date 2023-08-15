using System.Collections.Generic;
using System.IO;

using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Cecil;

#nullable enable

namespace Xamarin.Linker {
	public class ClassHandleRewriterStep : ConfigurationAwareStep {
		protected override string Name { get; } = "ClassHandleRewriter";
		protected override int ErrorCode { get; } = 2360;

		protected override void TryEndProcess ()
		{
			var app = Configuration.Application;

			app.SelectRegistrar ();

			if (app.Registrar == RegistrarMode.Static) {
				// with the static registrar selected, we can
				// rewrite the usage of class_ptr in NSObject
				// derived classes and (potentially) eliminate
				// static initializers.
				// but to do that, we have to register the
				// assemblies.
				Configuration.Target.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
				// Rewrite will do nothing if the optimization is off
				Configuration.Target.StaticRegistrar.Rewrite ();
			}
		}
	}
}
