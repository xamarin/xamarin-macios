using System;
using System.Collections.Generic;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class OpenTKTest {
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		// https://github.com/xamarin/xamarin-macios/issues/9724
		public void BeGone (AssemblyInfo info)
		{
			var assembly = info.Assembly;
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
