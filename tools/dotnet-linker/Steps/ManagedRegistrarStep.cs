using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Xamarin.Bundler;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Linker {
	public class ManagedRegistrarStep : ConfigurationAwareStep {
		protected override string Name { get; } = "ManagedRegistrar";
		protected override int ErrorCode { get; } = 2430;

		List<Exception> exceptions = new List<Exception> ();

		void AddException (Exception exception)
		{
			if (exceptions is null)
				exceptions = new List<Exception> ();
			exceptions.Add (exception);
		}

		protected override void TryProcess ()
		{
			base.TryProcess ();

			App.SelectRegistrar ();
			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;
		}

		protected override void TryEndProcess (out List<Exception>? exceptions)
		{
			base.TryEndProcess ();

			if (App.Registrar != RegistrarMode.ManagedStatic) {
				exceptions = null;
				return;
			}

			// Report back any exceptions that occurred during the processing.
			exceptions = this.exceptions;
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;
		}
	}
}
