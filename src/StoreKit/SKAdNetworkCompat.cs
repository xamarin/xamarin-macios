//
// SKAdNetworkCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#nullable enable

using System;
using System.ComponentModel;

using Foundation;

using ObjCRuntime;

#if TVOS && !NET
namespace StoreKit {
	[Obsolete ("Not usable from tvOS and will be removed in the future.")]
	[Unavailable (PlatformName.TvOS)]
	public class SKAdNetwork : NSObject {

		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		static readonly IntPtr class_ptr = Class.GetHandle ("SKAdNetwork");
		
		public override IntPtr ClassHandle { get { return class_ptr; } }
		
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected SKAdNetwork (NSObjectFlag t) : base (t) { }

		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal SKAdNetwork (IntPtr handle) : base (handle) { }

		[Obsolete ("Throws a 'NotSupportedException'.")]
		public static void RegisterAppForAdNetworkAttribution () => throw new NotSupportedException ();
	}
}
#endif // TVOS && !NET
