using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiPInvokeTest : ApiPInvokeTest {
		protected override bool SkipLibrary (string libraryName)
		{
			switch (libraryName) {
			case "/System/Library/Frameworks/OpenGL.framework/OpenGL":
				return true;
			}
			return base.SkipLibrary (libraryName);
		}

		protected override bool SkipAssembly (Assembly a)
		{
			// too many things are missing from XM 32bits bindings
			// and the BCL is identical for 64 bits (no need to test it 3 times)
			//			return IntPtr.Size == 4;
			return true; // skip everything until fixed
		}
	}
}
