using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using System.Text;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;
using Xamarin.Tuner;

namespace MonoTouch.Tuner {

	public class LinkerOptions {
		public IEnumerable<AssemblyDefinition> MainAssemblies { get; set; }
		public string OutputDirectory { get; set; }
		public LinkMode LinkMode { get; set; }
		public AssemblyResolver Resolver { get; set; }
		public IEnumerable<string> SkippedAssemblies { get; set; }
		public I18nAssemblies I18nAssemblies { get; set; }
		public bool LinkSymbols { get; set; }
		public bool LinkAway { get; set; }
		public bool Device { get; set; }
		public IList<string> ExtraDefinitions { get; set; }
		public bool DebugBuild { get; set; }
		public bool DumpDependencies { get; set; }
		internal PInvokeWrapperGenerator MarshalNativeExceptionsState { get; set; }
		internal RuntimeOptions RuntimeOptions { get; set; }
		public List<string> WarnOnTypeRef { get; set; }
		public bool RemoveRejectedTypes { get; set; }

		public MonoTouchLinkContext LinkContext { get; set; }
		public Target Target { get; set; }
		public Application Application { get { return Target.App; } }
	}

	static partial class Linker {

		public static void Process (LinkerOptions options, out MonoTouchLinkContext context, out List<AssemblyDefinition> assemblies)
		{
			var pipeline = CreatePipeline (options);

			foreach (var ad in options.MainAssemblies)
				pipeline.PrependStep (new MobileResolveMainAssemblyStep (ad, options.Application.Embeddinator));

			context = CreateLinkContext (options, pipeline);
			context.Resolver.AddSearchDirectory (options.OutputDirectory);

			if (options.DumpDependencies) {
				var prepareDependenciesDump = context.Annotations.GetType ().GetMethod ("PrepareDependenciesDump", new Type [1] { typeof (string) });
				if (prepareDependenciesDump is not null)
					prepareDependenciesDump.Invoke (context.Annotations, new object [1] { string.Format ("{0}{1}linker-dependencies.xml.gz", options.OutputDirectory, Path.DirectorySeparatorChar) });
			}

			Process (pipeline, context);

			assemblies = ListAssemblies (context);
		}

		static MonoTouchLinkContext CreateLinkContext (LinkerOptions options, Pipeline pipeline)
		{
			var context = new MonoTouchLinkContext (pipeline, options.Resolver);
			context.CoreAction = options.LinkMode == LinkMode.None ? AssemblyAction.Copy : AssemblyAction.Link;
			context.LinkSymbols = options.LinkSymbols;
			context.OutputDirectory = options.OutputDirectory;
			context.SetParameter ("debug-build", options.DebugBuild.ToString ());
			context.Target = options.Target;
			context.ExcludedFeatures = new [] { "remoting", "com", "sre" };
			context.SymbolWriterProvider = new CustomSymbolWriterProvider ();
			if (options.Application.Optimizations.StaticConstructorBeforeFieldInit == false)
				context.DisabledOptimizations |= CodeOptimizations.BeforeFieldInit;
			options.LinkContext = context;

			return context;
		}

		static SubStepDispatcher GetSubSteps ()
		{
			SubStepDispatcher sub = new SubStepDispatcher ();
			sub.Add (new MobileApplyPreserveAttribute ());
			sub.Add (new CoreRemoveSecurity ());
			sub.Add (new OptimizeGeneratedCodeSubStep ());
			sub.Add (new RemoveUserResourcesSubStep ());
			sub.Add (new RemoveAttributes ());
			// http://bugzilla.xamarin.com/show_bug.cgi?id=1408
			sub.Add (new RemoveCode ());
			sub.Add (new MarkNSObjects ());
			sub.Add (new PreserveSoapHttpClients ());
			sub.Add (new CoreHttpMessageHandler ());
			sub.Add (new InlinerSubStep ());
			sub.Add (new PreserveSmartEnumConversionsSubStep ());
			return sub;
		}

		static SubStepDispatcher GetPostLinkOptimizations (LinkerOptions options)
		{
			SubStepDispatcher sub = new SubStepDispatcher ();
			if (options.Application.Optimizations.ForceRejectedTypesRemoval == true)
				sub.Add (new RemoveRejectedTypesStep ());
			if (!options.DebugBuild) {
				sub.Add (new MetadataReducerSubStep ());
				if (options.Application.Optimizations.SealAndDevirtualize == true)
					sub.Add (new SealerSubStep ());
			}
			return sub;
		}

		static Pipeline CreatePipeline (LinkerOptions options)
		{
			var pipeline = new Pipeline ();

			pipeline.Append (new LoadReferencesStep ());

			if (options.I18nAssemblies != I18nAssemblies.None)
				pipeline.Append (new LoadI18nAssemblies (options.I18nAssemblies));

			// that must be done early since the XML files can "add" new assemblies [#15878]
			// and some of the assemblies might be (directly or referenced) SDK assemblies
			foreach (string definition in options.ExtraDefinitions)
				pipeline.Append (GetResolveStep (definition));

			if (options.LinkMode != LinkMode.None)
				pipeline.Append (new BlacklistStep ());

			if (options.WarnOnTypeRef.Count > 0)
				pipeline.Append (new PreLinkScanTypeReferenceStep (options.WarnOnTypeRef));

			pipeline.Append (new CustomizeIOSActions (options.LinkMode, options.SkippedAssemblies));

			// We need to store the Field attribute in annotations, since it may end up removed.
			pipeline.Append (new ProcessExportedFields ());

			// We remove incompatible bitcode from all assemblies, not only the linked assemblies.
			RemoveBitcodeIncompatibleCodeStep remove_incompatible_bitcode = null;
			if (options.Application.Optimizations.RemoveUnsupportedILForBitcode == true)
				remove_incompatible_bitcode = new RemoveBitcodeIncompatibleCodeStep (options);

			if (options.LinkMode != LinkMode.None) {
				pipeline.Append (new CoreTypeMapStep ());
				pipeline.Append (new RegistrarRemovalTrackingStep ());

				pipeline.Append (GetSubSteps ());

				pipeline.Append (new PreserveCode (options));

				pipeline.Append (new RemoveResources (options.I18nAssemblies)); // remove collation tables

				pipeline.Append (new MonoTouchMarkStep ());

				// We only want to remove from methods that aren't already linked away, so we need to do this
				// after the mark step. If we remove any incompatible code, we'll mark
				// the NotSupportedException constructor we need, so we need to do this before the sweep step.
				if (remove_incompatible_bitcode is not null)
					pipeline.Append (new SubStepDispatcher { remove_incompatible_bitcode });

				pipeline.Append (new MonoTouchSweepStep (options));
				pipeline.Append (new CleanStep ());

				pipeline.AppendStep (GetPostLinkOptimizations (options));

				pipeline.Append (new FixModuleFlags ());
			} else {
				SubStepDispatcher sub = new SubStepDispatcher () {
					new RemoveUserResourcesSubStep (),
				};
				if (options.Application.Optimizations.ForceRejectedTypesRemoval == true)
					sub.Add (new RemoveRejectedTypesStep ());
				if (remove_incompatible_bitcode is not null)
					sub.Add (remove_incompatible_bitcode);
				pipeline.Append (sub);
			}

			pipeline.Append (new ListExportedSymbols (options.MarshalNativeExceptionsState));

			pipeline.Append (new OutputStep ());

			// expect that changes can occur until it's all saved back to disk
			if (options.WarnOnTypeRef.Count > 0)
				pipeline.Append (new PostLinkScanTypeReferenceStep (options.WarnOnTypeRef));

			return pipeline;
		}

		static List<AssemblyDefinition> ListAssemblies (MonoTouchLinkContext context)
		{
			var list = new List<AssemblyDefinition> ();
			foreach (var assembly in context.GetAssemblies ()) {
				if (context.Annotations.GetAction (assembly) == AssemblyAction.Delete)
					continue;

				list.Add (assembly);
			}

			return list;
		}

		static ResolveFromXmlStep GetResolveStep (string filename)
		{
			filename = Path.GetFullPath (filename);

			if (!File.Exists (filename))
				throw new ProductException (2004, true, Errors.MX2004, filename);

			try {
				using (StreamReader sr = new StreamReader (filename)) {
					return new ResolveFromXmlStep (new XPathDocument (new StringReader (sr.ReadToEnd ())));
				}
			} catch (Exception e) {
				throw new ProductException (2005, true, e, Errors.MX2005, filename);
			}
		}
	}

	public class MonoTouchLinkContext : DerivedLinkContext {
		public MonoTouchLinkContext (Pipeline pipeline, AssemblyResolver resolver)
			: base (pipeline, resolver)
		{
		}
	}

	public class CustomizeIOSActions : CustomizeCoreActions {
		LinkMode link_mode;

		public CustomizeIOSActions (LinkMode mode, IEnumerable<string> skipped_assemblies)
			: base (mode == LinkMode.SDKOnly, skipped_assemblies)
		{
			link_mode = mode;
		}

		protected override bool IsLinked (AssemblyDefinition assembly)
		{
			if (link_mode == LinkMode.None)
				return false;

			return base.IsLinked (assembly);
		}

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (link_mode == LinkMode.None) {
				Annotations.SetAction (assembly, AssemblyAction.Copy);
				return;
			}

			try {
				base.ProcessAssembly (assembly);
			} catch (Exception e) {
				throw new ProductException (2103, true, e, Errors.MX2103, assembly.FullName, e);
			}
		}
	}
}
