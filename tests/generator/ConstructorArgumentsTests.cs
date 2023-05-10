#nullable enable

using System;
using System.Collections;
using NUnit.Framework;
using ObjCRuntime;

namespace GeneratorTests {

	[TestFixture]
	[Parallelizable (ParallelScope.All)]
	public class ConstructorArgumentsTests {

		[Test]
		public void GetCtorValuesNullVersion ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, "test");
			var values = args.GetCtorValues ();
#if NET
			Assert.AreEqual (2, values.Length, "Length");
			Assert.AreEqual ((byte)PlatformName.iOS, values[0], "Platform");
			Assert.AreEqual ("test", values[1], "Message");
#else
			Assert.AreEqual (3, values.Length, "Length");
			Assert.AreEqual ((byte) PlatformName.iOS, values [0], "Platform");
			Assert.AreEqual ((byte) 0xff, values [1], "Flag");
			Assert.AreEqual ("test", values [2], "Message");
#endif
		}

		[Test]
		public void GetCtorValuesNullBuild ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, 13, 0, "test");
			var values = args.GetCtorValues ();
#if NET
			Assert.AreEqual (4, values.Length, "Length");
			Assert.AreEqual ((byte)PlatformName.iOS, values[0], "Platform");
			Assert.AreEqual (13, values[1], "Major");
			Assert.AreEqual (0, values[2], "Minor");
			Assert.AreEqual ("test", values[3], "Message");
#else
			Assert.AreEqual (5, values.Length, "Length");
			Assert.AreEqual ((byte) PlatformName.iOS, values [0], "Platform");
			Assert.AreEqual (13, values [1], "Major");
			Assert.AreEqual (0, values [2], "Minor");
			Assert.AreEqual ((byte) 0xff, values [3], "Flag");
			Assert.AreEqual ("test", values [4], "Message");
#endif
		}

		[Test]
		public void GetCtorValuesFullVersion ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, 13, 0, 1, "test");
			var values = args.GetCtorValues ();
#if NET
			Assert.AreEqual (5, values.Length, "Length");
			Assert.AreEqual ((byte)PlatformName.iOS, values[0], "Platform");
			Assert.AreEqual (13, values[1], "Major");
			Assert.AreEqual (0, values[2], "Minor");
			Assert.AreEqual (1, values[3], "Build");
			Assert.AreEqual ("test", values[4], "Message");
#else
			Assert.AreEqual (6, values.Length, "Length");
			Assert.AreEqual ((byte) PlatformName.iOS, values [0], "Platform");
			Assert.AreEqual (13, values [1], "Major");
			Assert.AreEqual (0, values [2], "Minor");
			Assert.AreEqual (1, values [3], "Build");
			Assert.AreEqual ((byte) 0xff, values [4], "Flag");
			Assert.AreEqual ("test", values [5], "Message");
#endif
		}

		[Test]
		public void GetCtorTypesNullVersion ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, "test");
			var types = args.GetCtorTypes ();
#if NET
			Assert.AreEqual (2, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual(typeof(string), types[1], "Message");
#else
			Assert.AreEqual (3, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual (typeof (PlatformArchitecture), types [1], "Arch");
			Assert.AreEqual (typeof (string), types [2], "Message");
#endif
		}

		[Test]
		public void GetCtorTypesNullBuild ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, 13, 0, "test");
			var types = args.GetCtorTypes ();
#if NET
			Assert.AreEqual (4, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual (typeof (int), types [1], "Major");
			Assert.AreEqual (typeof (int), types [2], "Minor");
			Assert.AreEqual(typeof(string), types[3], "Message");
#else
			Assert.AreEqual (5, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual (typeof (int), types [1], "Major");
			Assert.AreEqual (typeof (int), types [2], "Minor");
			Assert.AreEqual (typeof (PlatformArchitecture), types [3], "Arch");
			Assert.AreEqual (typeof (string), types [4], "Message");
#endif
		}

		[Test]
		public void GetCtorTypesFullVersion ()
		{
			var args = new AttributeFactory.ConstructorArguments (PlatformName.iOS, 13, 0, 1, "test");
			var types = args.GetCtorTypes ();
#if NET
			Assert.AreEqual (5, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual (typeof (int), types [1], "Major");
			Assert.AreEqual (typeof (int), types [2], "Minor");
			Assert.AreEqual (typeof (int), types [3], "Build");
			Assert.AreEqual(typeof(string), types[4], "Message");
#else
			Assert.AreEqual (6, types.Length, "Length");
			Assert.AreEqual (typeof (PlatformName), types [0], "Platform");
			Assert.AreEqual (typeof (int), types [1], "Major");
			Assert.AreEqual (typeof (int), types [2], "Minor");
			Assert.AreEqual (typeof (int), types [3], "Build");
			Assert.AreEqual (typeof (PlatformArchitecture), types [4], "Arch");
			Assert.AreEqual (typeof (string), types [5], "Message");
#endif
		}

		class TryGetArgumentsData : IEnumerable {
			public IEnumerator GetEnumerator ()
			{
#if NET
				yield return new TestCaseData (
					new object [] { (byte)13, (byte)0 },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, null },
					new [] {  typeof(PlatformName), typeof (int), typeof (int), typeof (string) }
				);
				
				yield return new TestCaseData (
					new object [] { (byte)13, (byte)0 , (byte)1},
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (int)(byte)1,null },
					new [] {  typeof(PlatformName), typeof (int), typeof (int), typeof(int), typeof (string) }
				);
#else
				yield return new TestCaseData (
					new object [] { (byte) 13, (byte) 0 },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (byte) 0xff, null },
					new [] { typeof (PlatformName), typeof (int), typeof (int), typeof (PlatformArchitecture), typeof (string) }
				);
				yield return new TestCaseData (
					new object [] { (byte) 13, (byte) 0, (byte) 1 },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (int) (byte) 1, (byte) 0xff, null },
					new [] { typeof (PlatformName), typeof (int), typeof (int), typeof (int), typeof (PlatformArchitecture), typeof (string) }
				);
				yield return new TestCaseData (
					new object [] { (byte) 13, (byte) 0, true },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (byte) 2, null },
					new [] { typeof (PlatformName), typeof (int), typeof (int), typeof (PlatformArchitecture), typeof (string) }
				);

				yield return new TestCaseData (
					new object [] { (byte) 13, (byte) 0, (byte) 1, true },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (int) (byte) 1, (byte) 2, null },
					new [] { typeof (PlatformName), typeof (int), typeof (int), typeof (int), typeof (PlatformArchitecture), typeof (string) }
				);

				yield return new TestCaseData (
					new object [] { (byte) 13, (byte) 0, (byte) 1, (byte) 2 },
					PlatformName.iOS,
					new object? [] { (byte) PlatformName.iOS, (int) (byte) 13, (int) (byte) 0, (int) (byte) 1, (byte) 2, null },
					new [] { typeof (PlatformName), typeof (int), typeof (int), typeof (int), typeof (PlatformArchitecture), typeof (string) }
				);
#endif
			}
		}

		[TestCaseSource (typeof (TryGetArgumentsData))]
		public void TryGetCtorArguments (object [] arguments, PlatformName platformName, object [] expectedValues,
			Type [] expectedTypes)
		{
			var success = AttributeFactory.ConstructorArguments.TryGetCtorArguments (arguments, platformName,
				out var actualValues, out var actualTypes);
			Assert.True (success, "success");
			Assert.AreEqual (expectedValues!.Length, actualValues!.Length, "Values Length");
			for (int index = 0; index < expectedValues.Length; index++) {
				Assert.AreEqual (expectedValues [index], actualValues [index], $"Values [{index}]");
			}
			Assert.AreEqual (expectedTypes!.Length, actualTypes!.Length, "Types Length");
			for (int index = 0; index < expectedTypes.Length; index++) {
				Assert.AreEqual (expectedTypes [index], actualTypes [index], $"Types [{index}]");
			}
		}

		[Test]
		public void TryGetCtorArgumentsFail ()
		{
			var success = AttributeFactory.ConstructorArguments.TryGetCtorArguments (Array.Empty<object> (), PlatformName.iOS,
				out var actualValues, out var actualTypes);
			Assert.False (success, "success");
			Assert.Null (actualValues, "values");
			Assert.Null (actualTypes, "type");
		}
	}
}
