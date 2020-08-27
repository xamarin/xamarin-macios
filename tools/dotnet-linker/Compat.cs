// Compat.cs: might not be ideal but it eases code sharing with existing code during the initial implementation.
using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Linker;
using Xamarin.Utils;

namespace Xamarin.Bundler {
	public partial class Application {
		public LinkerConfiguration Configuration { get; private set; }

		public Application (LinkerConfiguration configuration)
		{
			this.Configuration = configuration;
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
				default:
					throw ErrorHelper.CreateError (177, Errors.MX0177 /* "Unknown platform: {0}. This usually indicates a bug; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case." */, Platform);
				}
			}
		}

		public void SelectRegistrar ()
		{
			if (Registrar == RegistrarMode.Default) {
				if (LinkMode == LinkMode.None && IsDefaultMarshalManagedExceptionMode) {
					Registrar = RegistrarMode.PartialStatic;
				} else {
					Registrar = RegistrarMode.Dynamic;
				}
			}
			Driver.Log (1, $"Registrar mode: {Registrar}");
		}

		public AssemblyBuildTarget LibMonoLinkMode {
			get { throw new NotImplementedException (); }
		}

		public AssemblyBuildTarget LibXamarinLinkMode {
			get { throw new NotImplementedException (); }
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

	public class DotNetLinkContext {
		public DotNetLinkContext ()
		{
			throw new NotImplementedException ();
		}

		public DotNetLinkContext (Pipeline pipeline, AssemblyResolver resolver)
		{
			throw new NotImplementedException ();
		}

		public AssemblyAction UserAction {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}

		public AnnotationStore Annotations {
			get {
				throw new NotImplementedException ();
			}
		}

		public AssemblyDefinition GetAssembly (string name)
		{
			throw new NotImplementedException ();
		}
	}

	public class Pipeline {

	}
}

namespace Xamarin.Linker {
	public class Profile {
		public LinkerConfiguration Configuration { get; private set; }

		public Profile (LinkerConfiguration config)
		{
			Configuration = config;
		}

		public bool IsProductAssembly (AssemblyDefinition assembly)
		{
			return assembly.Name.Name == Configuration.PlatformAssembly;
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
		public static Dictionary<IMetadataTokenProvider, object> GetCustomAnnotations (this AnnotationStore self, string name)
		{
			throw new NotImplementedException ();
		}
	}
}
