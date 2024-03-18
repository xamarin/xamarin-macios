using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using BenchmarkDotNet.Attributes;

using Bindings.Test;

namespace PerfTest {

	public class NativeArrays {

		NSNumber [] array;

		[Params (0, 1, 16, 256, 4096)]
		public int Length { get; set; }

		[GlobalSetup]
		public void Setup ()
		{
			array = new NSNumber [Length];
			for (int i = 0; i < Length; i++)
				array [i] = new NSNumber (i);
		}

		[Benchmark]
		public void Create ()
		{
			var native = CFArray.Create (array);
			CFString.ReleaseNative (native); // that's a `CFObject.CFRelease` with a null-check
		}

		int error;

		[Benchmark]
		public void ArrayFromHandleFunc ()
		{
			var native = CFArray.Create (array);
			var managed = CFArray.ArrayFromHandle<NSNumber> (native);
			if (managed.Length != array.Length)
				error++;
			CFString.ReleaseNative (native); // that's a `CFObject.CFRelease` with a null-check
		}
	}
}
