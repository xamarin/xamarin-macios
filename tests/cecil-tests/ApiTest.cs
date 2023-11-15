using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ApiTest {
		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblyDefinitions))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void ARConfiguration_GetSupportedVideoFormats (AssemblyInfo info)
		{
			// all subclasses of ARConfiguration must (re)export 'GetSupportedVideoFormats'
			var assembly = info.Assembly;
			List<string>? failures = null;

			if (!assembly.EnumerateTypes ((type) => type.Is ("ARKit", "ARConfiguration")).Any ())
				Assert.Ignore (); // This assembly doesn't contain ARKit.ARConfiguration

			var subclasses = assembly.EnumerateTypes ((type) => !type.Is ("ARKit", "ARConfiguration") && type.IsSubclassOf ("ARKit", "ARConfiguration"));
			Assert.That (subclasses, Is.Not.Empty, "At least some subclasses");

			foreach (var type in subclasses) {
				var method = type.Methods.SingleOrDefault (m => m.Name == "GetSupportedVideoFormats" && m.IsPublic && m.IsStatic && !m.HasParameters);
				if (method is null)
					AddFailure (ref failures, $"The type {type.FullName} does not implement the method GetSupportedVideoFormats.");
			}

			Assert.That (failures, Is.Null.Or.Empty, "All subclasses from ARConfiguration must explicitly implement GetSupportedVideoFormats.");
		}

		static void AddFailure (ref List<string>? failures, string failure)
		{
			if (failures is null)
				failures = new List<string> ();

			failures.Add (failure);
			Console.WriteLine (failure);
		}
	}
}
