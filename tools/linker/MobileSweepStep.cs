// Copyright 2012-2013, 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	// MobileMarkStep process a bit more data and that can be used to sweep
	// more metadata afterward. OTOH sweeping them without the extra marking
	// would produce invalid assemblies
	public class MobileSweepStep : SweepStep {

		public MobileSweepStep (bool sweepSymbols)
			: base (sweepSymbols)
		{
		}

		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.Context;
			}
		}

		public AssemblyAction CurrentAction { get; private set; }

		protected override void Process ()
		{
			base.Process ();

			var assemblies = Context.GetAssemblies ();
			foreach (var assembly in assemblies) {
				CurrentAction = Annotations.GetAction (assembly);
				switch (CurrentAction) {
				case AssemblyAction.Link:
				case AssemblyAction.Save:
					SweepAssembly (assembly);
					break;
				}
			}
		}

		protected virtual void SweepAssembly (AssemblyDefinition assembly)
		{
			SweepMainModule (assembly.MainModule);
		}

		protected void SweepMainModule (ModuleDefinition main)
		{
			// if we save (only or by linking) then unmarked exports (e.g. forwarders) must be cleaned
			// or they can point to nothing which will break later (e.g. when re-loading for stripping IL)
			// reference: https://bugzilla.xamarin.com/show_bug.cgi?id=36577
			if (main.HasExportedTypes)
				SweepCollection (main.ExportedTypes);
			// only when linking should we remove module references, if we (re)save the assembly then
			// the entrypoints (for p/invokes) will be required later
			// reference: https://bugzilla.xamarin.com/show_bug.cgi?id=35372
			if (main.HasModuleReferences && (CurrentAction == AssemblyAction.Link))
				SweepCollection (main.ModuleReferences);
		}

		protected override void InterfaceRemoved (TypeDefinition type, InterfaceImplementation iface)
		{
			base.InterfaceRemoved (type, iface);

			// The static registrar needs access to the interfaces for protocols, so keep them around.
			if (!LinkContext.ProtocolImplementations.TryGetValue (type, out var list))
				LinkContext.ProtocolImplementations [type] = list = new List<TypeDefinition> ();
			list.Add (iface.InterfaceType.Resolve ());
		}
	}
}