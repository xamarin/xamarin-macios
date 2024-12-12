// Compat.cs: might not be ideal but it eases code sharing with existing code during the initial implementation.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Bundler {
	public partial class Application {
		LinkerConfiguration? configuration;
		public LinkerConfiguration Configuration { get => configuration!; }
		public string? RuntimeConfigurationFile { get; set; }

		public Application (LinkerConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string ProductName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "Microsoft.iOS";
				case ApplePlatform.TVOS:
					return "Microsoft.tvOS";
				case ApplePlatform.WatchOS:
					return "Microsoft.watchOS";
				case ApplePlatform.MacOSX:
					return "Microsoft.macOS";
				case ApplePlatform.MacCatalyst:
					return "Microsoft.MacCatalyst";
				default:
					throw ErrorHelper.CreateError (177, Errors.MX0177 /* "Unknown platform: {0}. This usually indicates a bug; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case." */, Platform);
				}
			}
		}

		public void Initialize ()
		{
			// mSYM support is not implemented in the runtime on .NET 6 afaik
			EnableMSym = false;
		}

		public bool HasAnyDynamicLibraries {
			get { throw new NotImplementedException (); }
		}

		public string GetLibMono (AssemblyBuildTarget build_target)
		{
			throw new NotImplementedException ();
		}

		public string GetLibXamarin (AssemblyBuildTarget build_target)
		{
			throw new NotImplementedException ();
		}

		public Profile Profile {
			get {
				return Configuration.Profile;
			}
		}
	}

	public partial class Driver {
		public static string NAME {
			get { return "xamarin-bundler"; }
		}

		public static string GetArch32Directory (Application app)
		{
			throw new NotImplementedException ();
		}

		public static string GetArch64Directory (Application app)
		{
			throw new NotImplementedException ();
		}
	}

	// We can't make the linker use a LinkerContext subclass (DerivedLinkerContext), so we make DerivedLinkerContext
	// derive from this class, and then we redirect to the LinkerContext instance here.
	public class DotNetLinkContext {
		public LinkerConfiguration LinkerConfiguration;

		public DotNetLinkContext (LinkerConfiguration configuration)
		{
			this.LinkerConfiguration = configuration;
		}

		public AssemblyAction UserAction {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}

		public AnnotationStore Annotations {
			get {
				return LinkerConfiguration.Context.Annotations;
			}
		}

		public AssemblyDefinition GetAssembly (string name)
		{
			return LinkerConfiguration.Context.GetLoadedAssembly (name);
		}
	}

	public class Pipeline {

	}
}

namespace Xamarin.Linker {
	public class BaseProfile : Profile {
		public BaseProfile (LinkerConfiguration config)
			: base (config)
		{
		}
	}

	public class Profile {
		public LinkerConfiguration Configuration { get; private set; }

		public Profile (LinkerConfiguration config)
		{
			Configuration = config;
		}

		public Profile Current {
			get { return this; }
		}

		public string ProductAssembly {
			get { return Configuration.PlatformAssembly; }
		}

		public bool IsProductAssembly (string assemblyName)
		{
			return assemblyName == Configuration.PlatformAssembly;
		}

		public bool IsProductAssembly (AssemblyDefinition assembly)
		{
			return assembly.Name.Name == Configuration.PlatformAssembly;
		}

		public bool IsSdkAssembly (AssemblyDefinition assembly)
		{
			return Configuration.FrameworkAssemblies.Contains (Assembly.GetIdentity (assembly));
		}

		public bool IsSdkAssembly (string assemblyName)
		{
			return Configuration.FrameworkAssemblies.Contains (assemblyName);
		}
	}
}

namespace Mono.Linker {
	public static class LinkContextExtensions {
		public static void LogMessage (this LinkContext context, string messsage)
		{
			throw new NotImplementedException ();
		}
		public static IEnumerable<AssemblyDefinition> GetAssemblies (this LinkContext context)
		{
			return LinkerConfiguration.GetInstance (context).Assemblies;
		}

		static ConditionalWeakTable<AnnotationStore, Dictionary<string, Dictionary<IMetadataTokenProvider, object>>> custom_annotations = new ConditionalWeakTable<AnnotationStore, Dictionary<string, Dictionary<IMetadataTokenProvider, object>>> ();
		public static Dictionary<IMetadataTokenProvider, object> GetCustomAnnotations (this AnnotationStore self, string name)
		{
			var store = custom_annotations.GetOrCreateValue (self);
			if (!store.TryGetValue (name, out var dict))
				store [name] = dict = new Dictionary<IMetadataTokenProvider, object> ();
			return dict;
		}
	}
}
