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

		List<IMarkHandler> _markHandlers;
		List<IMarkHandler> MarkHandlers {
			get {
				if (_markHandlers == null) {
					var pipeline = typeof (LinkContext).GetProperty ("Pipeline").GetGetMethod ().Invoke (Context, null);
					_markHandlers = (List<IMarkHandler>) pipeline.GetType ().GetProperty ("MarkHandlers").GetValue (pipeline);
				}
				return _markHandlers;
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
			DumpSteps ();
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
			DumpSteps ();
			throw new InvalidOperationException ($"Could not insert {step} after {stepName} because {stepName} wasn't found.");
		}

		protected override void TryProcess ()
		{
			// Don't use --custom-step to load each step, because this assembly
			// is loaded into the current process once per --custom-step,
			// which makes it very difficult to share state between steps.

			// Load the list of assemblies loaded by the linker.
			// This would not be needed of LinkContext.GetAssemblies () was exposed to us.
			InsertBefore (new CollectAssembliesStep (), "MarkStep");

			var pre_mark_substeps = new DotNetSubStepDispatcher ();
			InsertBefore (pre_mark_substeps, "MarkStep");

			var post_sweep_substeps = new DotNetSubStepDispatcher ();
			InsertAfter (post_sweep_substeps, "SweepStep");

			if (Configuration.LinkMode != LinkMode.None) {
				MarkHandlers.Add (new PreserveBlockCodeHandler ());

				// We need to run the ApplyPreserveAttribute step even we're only linking sdk assemblies, because even
				// though we know that sdk assemblies will never have Preserve attributes, user assemblies may have
				// [assembly: LinkSafe] attributes, which means we treat them as sdk assemblies and those may have
				// Preserve attributes.
				MarkHandlers.Add (new DotNetMarkAssemblySubStepDispatcher (new ApplyPreserveAttribute ()));
				MarkHandlers.Add (new OptimizeGeneratedCodeHandler ());
				MarkHandlers.Add (new DotNetMarkAssemblySubStepDispatcher (new MarkNSObjects ()));
				MarkHandlers.Add (new PreserveSmartEnumConversionsHandler ());

				// This step could be run after Mark to avoid tracking all members:
				// https://github.com/xamarin/xamarin-macios/issues/11447
				pre_mark_substeps.Add (new CollectUnmarkedMembersSubStep ());
				pre_mark_substeps.Add (new StoreAttributesStep ());

				post_sweep_substeps.Add (new RemoveAttributesStep ());
			}

			InsertBefore (new ListExportedSymbols (null), "OutputStep");
			InsertBefore (new LoadNonSkippedAssembliesStep (), "OutputStep");
			InsertBefore (new ExtractBindingLibrariesStep (), "OutputStep");
			InsertBefore (new DotNetSubStepDispatcher (new RemoveUserResourcesSubStep ()), "OutputStep");
			Steps.Add (new RegistrarStep ());
			Steps.Add (new GenerateMainStep ());
			Steps.Add (new GenerateReferencesStep ());
			Steps.Add (new GatherFrameworksStep ());
			Steps.Add (new ComputeNativeBuildFlagsStep ());
			Steps.Add (new ComputeAOTArguments ());
			Steps.Add (new DoneStep ()); // Must be the last step.

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
