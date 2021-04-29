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

namespace Xamarin {

	public class SetupStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Setup";
		protected override int ErrorCode { get; } = 2300;

		List<IStep> _steps;
		public List<IStep> Steps {
			get {
				if (_steps == null) {
					var pipeline = typeof (LinkContext).GetProperty ("Pipeline").GetGetMethod ().Invoke (Context, null);
					_steps = (List<IStep>) pipeline.GetType ().GetField ("_steps", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (pipeline);
				}
				return _steps;
			}
		}


		protected override void TryProcess ()
		{
			Configuration.Write ();

			if (Configuration.Verbosity > 0) {
				DumpSteps ();
			}

			ErrorHelper.Platform = Configuration.Platform;
			Directory.CreateDirectory (Configuration.ItemsDirectory);
			Directory.CreateDirectory (Configuration.CacheDirectory);
		}

		void DumpSteps ()
		{
			Console.WriteLine ();
			Console.WriteLine ("Pipeline Steps:");
			foreach (var step in Steps) {
				Console.WriteLine ($"    {step}");
				if (step is SubStepsDispatcher) {
					var substeps = typeof (SubStepsDispatcher).GetField ("substeps", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue (step) as IEnumerable<ISubStep>;
					if (substeps != null) {
						foreach (var substep in substeps) {
							Console.WriteLine ($"        {substep}");
						}
					}
				}
			}
		}
	}
}
