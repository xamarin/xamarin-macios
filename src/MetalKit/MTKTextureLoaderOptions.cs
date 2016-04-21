//
// MTKTextureLoaderOptions.cs strong dictionary
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Foundation;
using XamCore.Metal;
using XamCore.ObjCRuntime;

namespace XamCore.MetalKit {
#if !COREBUILD
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	public partial class MTKTextureLoaderOptions : DictionaryContainer {

		public MTLTextureUsage? TextureUsage {
			get {
				var val = GetNUIntValue (MTKTextureLoaderKeys.TextureUsageKey);
				if (val != null)
					return (MTLTextureUsage)(uint) val;
				return null;
			}
			set {
				if (value.HasValue)
					SetNumberValue (MTKTextureLoaderKeys.TextureUsageKey, (nuint)(uint)value.Value);
				else
					RemoveValue (MTKTextureLoaderKeys.TextureUsageKey);
			}
		}

		public MTLCpuCacheMode? TextureCpuCacheMode {
			get {
				var val = GetNUIntValue (MTKTextureLoaderKeys.TextureCpuCacheModeKey);
				if (val != null)
					return (MTLCpuCacheMode)(uint) val;
				return null;
			}
			set {
				if (value.HasValue)
					SetNumberValue (MTKTextureLoaderKeys.TextureCpuCacheModeKey, (nuint)(uint)value.Value);
				else
					RemoveValue (MTKTextureLoaderKeys.TextureCpuCacheModeKey);
			}
		}
	}
#endif
}
#endif
