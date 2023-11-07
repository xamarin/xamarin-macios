// Copyright 2012-2013 Xamarin Inc.

using System;
using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoMac.Tuner {

	public class MacRemoveResources : RemoveResources {

		public MacRemoveResources (LinkerOptions options) :
			base (options.I18nAssemblies)
		{
		}

		protected LinkContext Context { get; private set; }

		public override void Process (LinkContext context)
		{
			Context = context;

			base.Process (context);

			// the resources are not part of the mobile profile (just the "classic" mono BCL)
			if (Profile.Current is MobileProfile)
				return;

			Process ("System", ProcessSystem);
			Process ("System.Drawing", ProcessSystemDrawing);
		}

		void Process (string assemblyName, Action<AssemblyDefinition> process)
		{
			AssemblyDefinition assembly;
			if (!Context.TryGetLinkedAssembly (assemblyName, out assembly))
				return;

			if (Context.Annotations.GetAction (assembly) == AssemblyAction.Link)
				process (assembly);
		}

		void Remove (AssemblyDefinition assembly, string extension)
		{
			var resources = assembly.MainModule.Resources;
			for (int i = 0; i < resources.Count; i++) {
				var resource = resources [i] as EmbeddedResource;
				if (resource is null)
					continue;
				if (resource.Name.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
					resources.RemoveAt (i--);
			}
		}

		void ProcessSystem (AssemblyDefinition assembly)
		{
			// bunch of .wav are kept as resources but only used by SystemSounds
			TypeDefinition type = assembly.MainModule.GetType ("System.Media.SystemSounds");
			if (!Context.Annotations.IsMarked (type))
				Remove (assembly, ".wav");
		}

		void ProcessSystemDrawing (AssemblyDefinition assembly)
		{
			// bunch of .ico are kept as resources but only used by SystemIcons
			TypeDefinition type = assembly.MainModule.GetType ("System.Drawing.SystemIcons");
			if (!Context.Annotations.IsMarked (type))
				Remove (assembly, ".ico");
		}
	}
}
