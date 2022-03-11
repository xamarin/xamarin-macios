using System;
using System.Collections.Generic;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class OpenTKTest {

#if NET
		[Ignore ("To be fixed after the move to an outside bot: https://github.com/xamarin/maccore/issues/2547.")]
#endif
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		// https://github.com/xamarin/xamarin-macios/issues/9724
		public void BeGone (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath)!;
			var found = new HashSet<string> ();
			foreach (var type in assembly.MainModule.Types) {
				if (type.Namespace?.StartsWith ("OpenTK", StringComparison.Ordinal) == true) {
					found.Add (type.FullName);
				}
			}

			Assert.That (found, Is.Empty);
		}
	}
}
