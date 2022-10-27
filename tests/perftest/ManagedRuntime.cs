using System;
using System.Runtime.InteropServices;

using BenchmarkDotNet.Attributes;

namespace PerfTest {

	public class ManagedRuntime {
		[Benchmark]
		public void AllocHGlobal ()
		{
			Marshal.FreeHGlobal (Marshal.AllocHGlobal (1));
		}
	}
}
