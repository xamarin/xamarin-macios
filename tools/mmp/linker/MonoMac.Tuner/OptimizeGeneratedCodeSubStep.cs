// Copyright 2012-2013,2016 Xamarin Inc. All rights reserved.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoMac.Tuner {
	
	public class OptimizeGeneratedCodeSubStep : CoreOptimizeGeneratedCode {
		
		public OptimizeGeneratedCodeSubStep (LinkerOptions options)
			: base (options)
		{
		}
	}
}