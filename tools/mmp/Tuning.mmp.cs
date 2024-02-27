using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;
using MonoMac.Tuner;
using MonoTouch.Tuner;
using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;
using Xamarin.Tuner;
using Xamarin.Utils;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMac.Tuner {

	public class LinkerOptions {
		public AssemblyDefinition MainAssembly { get; set; }
		public string OutputDirectory { get; set; }
		public bool LinkSymbols { get; set; }
		public LinkMode LinkMode { get; set; }
		public AssemblyResolver Resolver { get; set; }
		public IEnumerable<string> SkippedAssemblies { get; set; }
		public I18nAssemblies I18nAssemblies { get; set; }
		public IList<string> ExtraDefinitions { get; set; }
		internal PInvokeWrapperGenerator MarshalNativeExceptionsState { get; set; }
		internal RuntimeOptions RuntimeOptions { get; set; }
		public bool SkipExportedSymbolsInSdkAssemblies { get; set; }
		public MonoMacLinkContext LinkContext { get; set; }
		public Target Target { get; set; }
		public Application Application { get { return Target.App; } }
		public List<string> WarnOnTypeRef { get; set; }
	}

	public class MonoMacLinkContext : DerivedLinkContext {

		Dictionary<string, List<MethodDefinition>> pinvokes = new Dictionary<string, List<MethodDefinition>> ();

		public MonoMacLinkContext (Pipeline pipeline, AssemblyResolver resolver) : base (pipeline, resolver)
		{
		}

		public IDictionary<string, List<MethodDefinition>> PInvokeModules {
			get { return pinvokes; }
		}
	}

	static partial class Linker {

		public static void Process (LinkerOptions options, out MonoMacLinkContext context, out List<string> assemblies)
		{
			var pipeline = CreatePipeline (options);

			pipeline.PrependStep (new ResolveFromAssemblyStep (options.MainAssembly));

			context = CreateLinkContext (options, pipeline);
			context.Resolver.AddSearchDirectory (options.OutputDirectory);
			context.KeepTypeForwarderOnlyAssemblies = (Profile.Current is XamarinMacProfile);
			options.Target.LinkContext = (context as MonoMacLinkContext);

			Process (pipeline, context);

			assemblies = ListAssemblies (context);
		}

		static MonoMacLinkContext CreateLinkContext (LinkerOptions options, Pipeline pipeline)
		{
			var context = new MonoMacLinkContext (pipeline, options.Resolver);
			context.CoreAction = AssemblyAction.Link;
			context.LinkSymbols = options.LinkSymbols;
			if (options.LinkSymbols) {
				context.SymbolReaderProvider = new DefaultSymbolReaderProvider ();
				context.SymbolWriterProvider = new DefaultSymbolWriterProvider ();
			}
			context.OutputDirectory = options.OutputDirectory;
			context.Target = options.Target;
			options.LinkContext = context;
			return context;
		}

		static Pipeline CreatePipeline (LinkerOptions options)
		{
			var pipeline = new Pipeline ();

			pipeline.Append (options.LinkMode == LinkMode.None ? new LoadOptionalReferencesStep () : new LoadReferencesStep ());

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

			pipeline.Append (new CustomizeMacActions (options.LinkMode, options.SkippedAssemblies));

			// We need to store the Field attribute in annotations, since it may end up removed.
			pipeline.Append (new ProcessExportedFields ());

			if (options.LinkMode != LinkMode.None) {
				pipeline.Append (new CoreTypeMapStep ());

				pipeline.Append (GetSubSteps ());

				pipeline.Append (new CorePreserveCode (options.I18nAssemblies));
				pipeline.Append (new PreserveCrypto ());

				pipeline.Append (new MonoMacMarkStep ());
				pipeline.Append (new MacRemoveResources (options));
				pipeline.Append (new CoreSweepStep (options.LinkSymbols));
				pipeline.Append (new CleanStep ());

				pipeline.Append (new MonoMacNamespaces ());
				pipeline.Append (new RemoveSelectors ());

				pipeline.Append (new RegenerateGuidStep ());
			} else {
				SubStepDispatcher sub = new SubStepDispatcher () {
					new RemoveUserResourcesSubStep ()
				};
				pipeline.Append (sub);
			}

			pipeline.Append (new ListExportedSymbols (options.MarshalNativeExceptionsState, options.SkipExportedSymbolsInSdkAssemblies));

			pipeline.Append (new OutputStep ());

			// expect that changes can occur until it's all saved back to disk
			if (options.WarnOnTypeRef.Count > 0)
				pipeline.Append (new PostLinkScanTypeReferenceStep (options.WarnOnTypeRef));

			return pipeline;
		}

		static SubStepDispatcher GetSubSteps ()
		{
			SubStepDispatcher sub = new SubStepDispatcher ();
			sub.Add (new MobileApplyPreserveAttribute ());
			sub.Add (new OptimizeGeneratedCodeSubStep ());
			sub.Add (new RemoveUserResourcesSubStep ());
			// OptimizeGeneratedCodeSubStep and RemoveUserResourcesSubStep needs [GeneratedCode] so it must occurs before RemoveAttributes
			sub.Add (new CoreRemoveAttributes ());

			sub.Add (new CoreHttpMessageHandler ());
			sub.Add (new MarkNSObjects ());

			sub.Add (new CoreRemoveSecurity ());
			sub.Add (new PreserveSmartEnumConversionsSubStep ());

			return sub;
		}

		static List<string> ListAssemblies (LinkContext context)
		{
			var list = new List<string> ();
			foreach (var assembly in context.GetAssemblies ()) {
				if (context.Annotations.GetAction (assembly) == AssemblyAction.Delete)
					continue;

				list.Add (GetFullyQualifiedName (assembly));

				Driver.Log (1, "Loaded assembly: {0}", assembly.MainModule.FileName);
			}

			return list;
		}

		static string GetFullyQualifiedName (AssemblyDefinition assembly)
		{
			return assembly.MainModule.FileName;
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


	public class CustomizeMacActions : CustomizeCoreActions {
		LinkMode link_mode;

		public CustomizeMacActions (LinkMode mode, IEnumerable<string> skipped_assemblies)
			: base (mode == LinkMode.SDKOnly, skipped_assemblies)
		{
			link_mode = mode;
		}

		protected override bool IsLinked (AssemblyDefinition assembly)
		{
			if (link_mode == LinkMode.None)
				return false;

			if (link_mode == LinkMode.Platform)
				return Profile.IsProductAssembly (assembly);

			return base.IsLinked (assembly);
		}

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			switch (link_mode) {
			case LinkMode.Platform:
				if (!Profile.IsProductAssembly (assembly))
					Annotations.SetAction (assembly, AssemblyAction.Copy);
				break;
			case LinkMode.None:
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

	class LoadOptionalReferencesStep : LoadReferencesStep {
		HashSet<AssemblyNameDefinition> _references = new HashSet<AssemblyNameDefinition> ();

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			ProcessAssemblyReferences (assembly);
		}

		void ProcessAssemblyReferences (AssemblyDefinition assembly)
		{
			if (_references.Contains (assembly.Name))
				return;

			_references.Add (assembly.Name);

			foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences) {
				try {
					var asm = Context.Resolve (reference);
					if (asm is null) {
						ErrorHelper.Warning (2013, Errors.MM2013, reference.FullName, assembly.Name.FullName);
					} else {
						ProcessReferences (asm);
					}
				} catch (AssemblyResolutionException fnfe) {
					ErrorHelper.Warning (2013, fnfe, Errors.MM2013, fnfe.AssemblyReference.FullName, assembly.Name.FullName);
				}
			}
		}
	}

}
