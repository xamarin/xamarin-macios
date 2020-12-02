using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Mono.Linker;
using Mono.Linker.Steps;

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

		void InsertBefore (IStep step, string stepName)
		{
			for (int i = 0; i < Steps.Count; i++) {
				if (Steps [i].GetType ().Name == stepName) {
					Steps.Insert (i, step);
					return;
				}
			}
			throw new InvalidOperationException ($"Could not insert {step} before {stepName} because {stepName} wasn't found.");
		}

		void InsertAfter (IStep step, string stepName)
		{
			for (int i = 0; i < Steps.Count;) {
				if (Steps [i++].GetType ().Name == stepName) {
					Steps.Insert (i, step);
					return;
				}
			}
			throw new InvalidOperationException ($"Could not insert {step} after {stepName} because {stepName} wasn't found.");
		}

		protected override void TryProcess ()
		{
			// Don't use --custom-step to load each step, because this assembly
			// is loaded into the current process once per --custom-step,
			// which makes it very difficult to share state between steps.

			// Load the list of assemblies loaded by the linker.
			// This would not be needed of LinkContext.GetAssemblies () was exposed to us.
			InsertAfter (new CollectAssembliesStep (), "LoadReferencesStep");

			var pre_dynamic_dependency_lookup_substeps = new DotNetSubStepDispatcher ();
			InsertBefore (pre_dynamic_dependency_lookup_substeps, "DynamicDependencyLookupStep");

			var prelink_substeps = new DotNetSubStepDispatcher ();
			InsertAfter (prelink_substeps, "RemoveSecurityStep");

			var post_sweep_substeps = new DotNetSubStepDispatcher ();
			InsertAfter (post_sweep_substeps, "SweepStep");

			if (Configuration.LinkMode != LinkMode.None) {
				pre_dynamic_dependency_lookup_substeps.Add (new PreserveBlockCodeSubStep ());

				// We need to run the ApplyPreserveAttribute step even we're only linking sdk assemblies, because even
				// though we know that sdk assemblies will never have Preserve attributes, user assemblies may have
				// [assembly: LinkSafe] attributes, which means we treat them as sdk assemblies and those may have
				// Preserve attributes.
				prelink_substeps.Add (new ApplyPreserveAttribute ());
				prelink_substeps.Add (new OptimizeGeneratedCodeSubStep ());
				prelink_substeps.Add (new MarkNSObjects ());
				prelink_substeps.Add (new PreserveSmartEnumConversionsSubStep ());
				prelink_substeps.Add (new CollectUnmarkedMembersSubStep ());
				prelink_substeps.Add (new StoreAttributesStep ());

				post_sweep_substeps.Add (new RemoveAttributesStep ());
			}

			Steps.Add (new ListExportedSymbols (null));
			Steps.Add (new LoadNonSkippedAssembliesStep ());
			Steps.Add (new ExtractBindingLibrariesStep ());
			Steps.Add (new RegistrarStep ());
			Steps.Add (new GenerateMainStep ());
			Steps.Add (new GenerateReferencesStep ());
			Steps.Add (new GatherFrameworksStep ());

			Configuration.Write ();

			if (Configuration.Verbosity > 0) {
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

			ErrorHelper.Platform = Configuration.Platform;
			Directory.CreateDirectory (Configuration.ItemsDirectory);
			Directory.CreateDirectory (Configuration.CacheDirectory);
		}
	}
}
