// Copyright 2013 Xamarin Inc. All rights reserved.

using Mono.Tuner;

namespace Xamarin.Linker {

	// note: XamMac 1.x does not offer the mobile profile so we need a base class to MobileProfile

	public abstract class BaseProfile : Profile {

		// return assembly name without extension (.dll)
		public abstract string ProductAssembly { get; }
	}
}
