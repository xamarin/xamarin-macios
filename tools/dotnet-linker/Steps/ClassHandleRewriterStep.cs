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
				Configuration.Target.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
				Configuration.Target.StaticRegistrar.Rewrite ();
			}
		}
	}
}
