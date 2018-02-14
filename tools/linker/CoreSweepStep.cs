// Copyright 2018 Microsoft Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	// CoreSweepStep is shared between Xamarin.Mac and Xamarin.iOS
	public class CoreSweepStep : MobileSweepStep {

		public CoreSweepStep (bool sweepSymbols)
			: base (sweepSymbols)
		{
		}

		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.Context;
			}
		}

		protected override void InterfaceRemoved (TypeDefinition type, InterfaceImplementation iface)
		{
			base.InterfaceRemoved (type, iface);

			// The static registrar needs access to the interfaces for protocols, so keep them around.
			if (!LinkContext.ProtocolImplementations.TryGetValue (type, out var list))
				LinkContext.ProtocolImplementations [type] = list = new List<TypeDefinition> ();
			list.Add (iface.InterfaceType.Resolve ());
		}

		protected override void ElementRemoved (IMetadataTokenProvider element)
		{
			base.ElementRemoved (element);

			if (element is TypeDefinition td)
				LinkContext.AddLinkedAwayType (td);
		}
	}
}
