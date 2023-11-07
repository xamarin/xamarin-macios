using System;
using System.ComponentModel;
using System.Reflection;

using NUnit.Framework;

using Foundation;

namespace ObjCRuntime {
	public class Trampolines {

		internal delegate void DInnerBlock (IntPtr block, int magic_number);

		internal class NIDInnerBlock { }

		static internal class SDInnerBlock {
			// not preserved by attributes
			static internal readonly DInnerBlock Handler = Invoke;

			[MonoPInvokeCallback (typeof (DInnerBlock))]
			static internal void Invoke (IntPtr block, int magic_number)
			{
			}
		}

		static internal class SDInnerBlock_Misnamed {
			// not preserved by attributes
			static internal readonly DInnerBlock MisHandler = Invoke;

			[MonoPInvokeCallback (typeof (DInnerBlock))]
			static internal void Invoke (IntPtr block, int magic_number)
			{
			}
		}
	}
}
namespace LinkAll {

	public class CustomConverter : TypeConverter {
		// note: the default ctor will be preserved by the linker because it's used in a [TypeConverter] attribute
	}

	[TypeConverter (typeof (BooleanConverter))]
	public class BuiltInConverter {
	}

	[TypeConverter (typeof (CustomConverter))]
	class TypeDescriptorTest {
	}

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class CommonLinkAllTest {
		string WorkAroundLinkerHeuristics { get { return ""; } }

		[Test]
		public void BindingsAndBeforeInitField ()
		{
			ObjCRuntime.Trampolines.SDInnerBlock.Invoke (IntPtr.Zero, 0);
			var fields = Type.GetType ("ObjCRuntime.Trampolines+SDInnerBlock" + WorkAroundLinkerHeuristics).GetFields (BindingFlags.NonPublic | BindingFlags.Static);
			Assert.That (fields.Length, Is.EqualTo (1), "one");
			Assert.That (fields [0].Name, Is.EqualTo ("Handler"), "Name");
		}

		[Test]
		public void BindingsAndBeforeInitField_2 ()
		{
			ObjCRuntime.Trampolines.SDInnerBlock_Misnamed.Invoke (IntPtr.Zero, 0);
			var fields = Type.GetType ("ObjCRuntime.Trampolines+SDInnerBlock_Misnamed" + WorkAroundLinkerHeuristics).GetFields (BindingFlags.NonPublic | BindingFlags.Static);
			Assert.That (fields.Length, Is.EqualTo (0), "zero");
		}

		[Test]
		public void TypeConverter_BuiltIn ()
		{
			Assert.NotNull (TypeDescriptor.GetConverter (new BuiltInConverter ()), "BuiltInConverter");

			string name = (typeof (BuiltInConverter).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
#if NET
			var typename = $"System.ComponentModel.BooleanConverter, System.ComponentModel.TypeConverter, Version={typeof (int).Assembly.GetName ().Version}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
#else
			var typename = "System.ComponentModel.BooleanConverter, System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
#endif
			Assert.That (name, Is.EqualTo (typename), "ConverterTypeName");
		}

		[Test]
		public void TypeConverter_Custom ()
		{
			Assert.NotNull (TypeDescriptor.GetConverter (new TypeDescriptorTest ()), "TypeDescriptorTest");

			string name = (typeof (TypeDescriptorTest).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
#if NET
			var typename = "LinkAll.CustomConverter, link all, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
#else
			var typename = "LinkAll.CustomConverter, link all, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
#endif
			Assert.That (name, Is.EqualTo (typename), "ConverterTypeName");
		}

		[Test]
		public void ConstantsVersion_4859 ()
		{
			// Check that the Makefile generated a valid version number, e.g. "12.3." was not
			// reference: https://github.com/xamarin/xamarin-macios/issues/4859
			Assert.True (Version.TryParse (ObjCRuntime.Constants.Version, out var _), "Version");
		}
	}
}
