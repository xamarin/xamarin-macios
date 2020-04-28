#if true

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using MonoTouch.Tuner;
using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;

namespace Xamarin {

	public class ConfigurationAwareStep : BaseStep {
		public LinkerConfiguration Configuration {
			get { return LinkerConfiguration.Instance; }
		}
	}
	
	public class SetupStep : ConfigurationAwareStep {

		List<IStep> _steps;

		List<IStep> Steps {
			get {
				if (_steps == null) {
					var pipeline = typeof (LinkContext).GetProperty ("Pipeline").GetGetMethod ().Invoke (Context, null);
					_steps = (List<IStep>) pipeline.GetType ().GetField ("_steps", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (pipeline);
				}
				return _steps;
			}
		}

		void InsertAfter (IStep step, string stepName)
		{
			for (int i = 0; i < Steps.Count;) {
				if (Steps [i++].GetType ().Name == stepName) {
					Steps.Insert (i, step);
					return;
				}
			}
			throw new InvalidOperationException ($"Could not insert {step} after {stepName}.");
		}

		protected override void Process ()
		{
			// we need to store the Field attribute in annotations, since it may end up removed.
			InsertAfter (new ProcessExportedFields (), "TypeMapStep");

			switch (Configuration.LinkMode) {
			case LinkMode.None:
				// remove about everything if we're not linking
				for (int i = Steps.Count - 1; i >= 0; i--) {
					switch (Steps [i].GetType ().Name) {
					// remove well-known steps as removing all but needed steps would make --custom-step unusable :|
					case "BlacklistStep":
					case "PreserveDependencyLookupStep":
					case "TypeMapStep":
					case "BodySubstituterStep":
					case "RemoveSecurityStep":
					case "RemoveUnreachableBlocksStep":
					case "MarkStep":
					case "SweepStep":
					case "CodeRewriterStep":
					case "CleanStep":
					case "RegenerateGuidStep":
					case "ClearInitLocalsStep":
					case "SealerStep":
						Steps.RemoveAt (i);
						break;
					}
				}
				// Mark and Sweep steps are where we normally update CopyUsed to Copy
				// so we need something else, more lightweight, to do this job
				InsertAfter (new DoNotLinkStep (), "LoadReferencesStep");
				break;
			case LinkMode.SDKOnly:
				for (int i = Steps.Count - 1; i >= 0; i--) {
					switch (Steps [i].GetType ().Name) {
					// FIXME: it's so noisy that il makes the log viewer hang :(
					case "RemoveUnreachableBlocksStep":
						Steps.RemoveAt (i);
						break;
					}
				}
				// platform assemblies (and friends) are linked along with the BCL
				InsertAfter (new LinkSdkStep (), "LoadReferencesStep");

				var subs = new MobileSubStepDispatcher ();
				// only our _old_ [Preserve] code is needed, other stuff is handled differently
				subs.Add (new ApplyPreserveAttribute ());
				// CoreRemoveSecurity does not hit any extra case
				// the old #28918 bug should not happen anymore (types not present)
				// https://xamarin.github.io/bugzilla-archives/28/28918/bug.html
				// subs.Add (new Mono.Tuner.CoreRemoveSecurity ());

				// subs.Add (new OptimizeGeneratedCodeSubStep ());
				// subs.Add (new RemoveUserResourcesSubStep ());
				// subs.Add (new RemoveAttributes ());
				// // http://bugzilla.xamarin.com/show_bug.cgi?id=1408
				// subs.Add (new RemoveCode ());

				subs.Add (new MarkNSObjects ());

				// subs.Add (new PreserveSoapHttpClients ());
				// subs.Add (new CoreHttpMessageHandler ());
				// subs.Add (new InlinerSubStep ());

				subs.Add (new PreserveSmartEnumConversionsSubStep ());

				InsertAfter (subs, "RemoveSecurityStep");
				break;
			case LinkMode.All:
				break;
			case LinkMode.Platform:
				throw new NotSupportedException ();
			}

			if (Configuration.InsertTimestamps) {
				// note: some steps (e.g. BlacklistStep) dynamically adds steps to the pipeline
				for (int i = Steps.Count - 1; i >= 0; i--) {
					Steps.Insert (i + 1, new TimeStampStep (Steps [i].ToString ()));
				}
				Steps.Insert (0, new TimeStampStep ("Start"));
			}

			if (Configuration.InsaneVerbosity) {
				Configuration.Write ();
				Console.WriteLine ("Pipeline Steps:");
				foreach (var step in Steps) {
					Console.WriteLine ($"    {step}");
				}
			}
		}
	}

	public class DoNotLinkStep : ConfigurationAwareStep {

		Dictionary<string,AssemblyDefinition> defs = new Dictionary<string,AssemblyDefinition> ();
		HashSet<string> refs = new HashSet<string> ();

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			defs.Add (assembly.Name.Name, assembly);
			foreach (var m in assembly.Modules) {
				if (m.HasAssemblyReferences) {
					foreach (var reference in m.AssemblyReferences) {
						refs.Add (reference.Name);
					}
				}
			}
		}

		protected override void EndProcess ()
		{
			// promotion time! anything that is referenced must be Copy (not CopyUsed)
			foreach (var r in refs) {
				if (defs.TryGetValue (r, out var a)) {
					if (Annotations.GetAction (a) == AssemblyAction.CopyUsed)
						Annotations.SetAction (a, AssemblyAction.Copy);
				} else {
					Console.WriteLine ($"Could not find reference {r}");
				}
			}

			if (Configuration.InsaneVerbosity) {
				// in theory both numbers should be identical since we're not linking
				// but we can see references (from Xamarin.iOS.dll) to older versions of some assemblies
				Console.WriteLine ($"Assemblies ({defs.Count}) and references ({refs.Count} unique):");
				foreach (var a in defs.Values) {
					Console.WriteLine ($"    {a.Name}");
					foreach (var r in a.MainModule.AssemblyReferences)
						Console.WriteLine ($"        {r}");
				}
			}
		}
	}

	public class LinkSdkStep : ConfigurationAwareStep {

		// TODO move into some 'app data' object
		public static HashSet<AssemblyDefinition> defs = new HashSet<AssemblyDefinition> ();
		public static AssemblyDefinition PlatformAssembly;

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			// lack of `Context.GetAssemblies` API
			defs.Add (assembly);

			var name = assembly.Name.Name;
			switch (name) {
			case "Xamarin.Forms.Platform.iOS": // special case
				Annotations.SetAction (assembly, AssemblyAction.Link);
				break;
			case string _ when name == Configuration.PlatformAssembly:
				PlatformAssembly = assembly;
				break;
			}
		}
	}

	public class TimeStampStep : IStep {
		readonly string message;

		public TimeStampStep (string message)
		{
			this.message = message;
		}

		public void Process (LinkContext context)
		{
			Console.WriteLine ($"{DateTime.UtcNow}: {message}");
		}
	}
}

#endif

