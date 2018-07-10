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
		public TargetFramework TargetFramework { get; set; }
		public string Architecture { get; set; }
		internal PInvokeWrapperGenerator MarshalNativeExceptionsState { get; set; }
		internal RuntimeOptions RuntimeOptions { get; set; }
		public bool SkipExportedSymbolsInSdkAssemblies { get; set; }
		public Target Target { get; set; }
		public Application Application { get { return Target.App; } }

		public static I18nAssemblies ParseI18nAssemblies (string i18n)
		{
			var assemblies = I18nAssemblies.None;

			foreach (var part in i18n.Split (',')) {
				var assembly = part.Trim ();
				if (string.IsNullOrEmpty (assembly))
					continue;

				try {
					assemblies |= (I18nAssemblies) Enum.Parse (typeof (I18nAssemblies), assembly, true);
				} catch {
					throw new FormatException ("Unknown value for i18n: " + assembly);
				}
			}

			return assemblies;
		}
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

	class Linker {

		public static void Process (LinkerOptions options, out LinkContext context, out List<string> assemblies)
		{
			var pipeline = CreatePipeline (options);

			pipeline.PrependStep (new ResolveFromAssemblyStep (options.MainAssembly));

			context = CreateLinkContext (options, pipeline);
			context.Resolver.AddSearchDirectory (options.OutputDirectory);
			context.KeepTypeForwarderOnlyAssemblies = (Profile.Current is XamarinMacProfile);
			options.Target.LinkContext = (context as MonoMacLinkContext);

			try {
				pipeline.Process (context);
			} catch (AssemblyResolutionException fnfe) {
				throw new MonoMacException (2002, true, fnfe, fnfe.Message);
			} catch (AggregateException) {
				throw;
			} catch (MonoMacException) {
				throw;
			} catch (ResolutionException re) {
				TypeReference tr = (re.Member as TypeReference);
				IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
				throw new MonoMacException (2002, true, re, "Failed to resolve \"{0}\" reference from \"{1}\"", re.Member, scope);
			} catch (XmlResolutionException ex) {
				throw new MonoMacException (2017, true, ex, "Could not process XML description: {0}", ex?.InnerException?.Message ?? ex.Message);
			} catch (Exception e) {
				var message = new StringBuilder ();
				if (e.Data.Count > 0) {
					message.AppendLine ();
					var m = e.Data ["MethodDefinition"] as string;
					if (m != null)
						message.AppendLine ($"\tMethod: `{m}`");
					var t = e.Data ["TypeReference"] as string;
					if (t != null)
						message.AppendLine ($"\tType: `{t}`");
					var a = e.Data ["AssemblyDefinition"] as string;
					if (a != null)
						message.AppendLine ($"\tAssembly: `{a}`");
				}
				message.Append ($"Reason: {e.Message}");
				throw new MonoMacException (2001, true, e, "Could not link assemblies. {0}", message);
			}

			assemblies = ListAssemblies (context);
		}

		static LinkContext CreateLinkContext (LinkerOptions options, Pipeline pipeline)
		{
			var context = new MonoMacLinkContext (pipeline, options.Resolver);
			context.CoreAction = AssemblyAction.Link;
			context.LinkSymbols = options.LinkSymbols;
			if (options.LinkSymbols) {
				context.SymbolReaderProvider = new DefaultSymbolReaderProvider ();
				context.SymbolWriterProvider = new DefaultSymbolWriterProvider ();
			}
			context.OutputDirectory = options.OutputDirectory;
			context.StaticRegistrar = options.Target.StaticRegistrar;
			context.Target = options.Target;
			context.ExcludedFeatures = new [] { "remoting", "com" };
			return context;
		}

		static Pipeline CreatePipeline (LinkerOptions options)
		{
			var pipeline = new Pipeline ();

			pipeline.AppendStep (options.LinkMode == LinkMode.None ? new LoadOptionalReferencesStep () : new LoadReferencesStep ());

			if (options.I18nAssemblies != I18nAssemblies.None)
				pipeline.AppendStep (new LoadI18nAssemblies (options.I18nAssemblies));

			// that must be done early since the XML files can "add" new assemblies [#15878]
			// and some of the assemblies might be (directly or referenced) SDK assemblies
			foreach (string definition in options.ExtraDefinitions)
				pipeline.AppendStep (GetResolveStep (definition));

			if (options.LinkMode != LinkMode.None)
				pipeline.AppendStep (new BlacklistStep ());

			pipeline.AppendStep (new CustomizeMacActions (options.LinkMode, options.SkippedAssemblies));

			// We need to store the Field attribute in annotations, since it may end up removed.
			pipeline.AppendStep (new ProcessExportedFields ());

			if (options.LinkMode != LinkMode.None) {
				pipeline.AppendStep (new CoreTypeMapStep ());

				var subdispatcher = new SubStepDispatcher {
					new ApplyPreserveAttribute (),
					new OptimizeGeneratedCodeSubStep (options),
					new RemoveUserResourcesSubStep (),
					new CoreRemoveAttributes (),
					new CoreHttpMessageHandler (options),
					new MarkNSObjects (),
				};
				// CoreRemoveSecurity can modify non-linked assemblies
				// but the conditions for this cannot happen if only the platform assembly is linked
				if (options.LinkMode != LinkMode.Platform)
					subdispatcher.Add (new CoreRemoveSecurity ());
				pipeline.AppendStep (subdispatcher);

				pipeline.AppendStep (new MonoMacPreserveCode (options));
				pipeline.AppendStep (new PreserveCrypto ());

				pipeline.AppendStep (new MonoMacMarkStep ());
				pipeline.AppendStep (new MacRemoveResources (options));
				pipeline.AppendStep (new CoreSweepStep (options.LinkSymbols));
				pipeline.AppendStep (new CleanStep ());

				pipeline.AppendStep (new MonoMacNamespaces ());
				pipeline.AppendStep (new RemoveSelectors ());

				pipeline.AppendStep (new RegenerateGuidStep ());
			} else {
				SubStepDispatcher sub = new SubStepDispatcher () {
					new RemoveUserResourcesSubStep ()
				};
				pipeline.AppendStep (sub);
			}

			pipeline.AppendStep (new ListExportedSymbols (options.MarshalNativeExceptionsState, options.SkipExportedSymbolsInSdkAssemblies));

			pipeline.AppendStep (new OutputStep ());

			return pipeline;
		}

		static List<string> ListAssemblies (LinkContext context)
		{
			var list = new List<string> ();
			foreach (var assembly in context.GetAssemblies ()) {
				if (context.Annotations.GetAction (assembly) == AssemblyAction.Delete)
					continue;

				list.Add (GetFullyQualifiedName (assembly));
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
				throw new MonoMacException (2004, true, "Extra linker definitions file '{0}' could not be located.", filename);
			
			try {
				using (StreamReader sr = new StreamReader (filename)) {
					return new ResolveFromXmlStep (new XPathDocument (new StringReader (sr.ReadToEnd ())));
				}
			}
			catch (Exception e) {
				throw new MonoMacException (2005, true, e, "Definitions from '{0}' could not be parsed.", filename);
			}
		}
	}


	public class CustomizeMacActions : CustomizeActions
	{
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
			}
			catch (Exception e) {
				throw new MonoMacException (2103, true, e, $"Error processing assembly '{assembly.FullName}': {e}");
			}
		}
	}

	class LoadOptionalReferencesStep : LoadReferencesStep
	{
		HashSet<AssemblyNameDefinition> _references = new HashSet<AssemblyNameDefinition> ();

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			ProcessReferences (assembly);
		}

		void ProcessReferences (AssemblyDefinition assembly)
		{
			if (_references.Contains (assembly.Name))
				return;

			_references.Add (assembly.Name);

			foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences) {
				try {
					ProcessReferences (Context.Resolve (reference));
				} catch (AssemblyResolutionException fnfe) {
					ErrorHelper.Warning (2013, fnfe, "Failed to resolve the reference to \"{0}\", referenced in \"{1}\". The app will not include the referenced assembly, and may fail at runtime.", fnfe.AssemblyReference.FullName, assembly.Name.FullName);
				}
			}
		}
	}

}
