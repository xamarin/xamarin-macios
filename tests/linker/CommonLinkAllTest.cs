using System;
using System.ComponentModel;

using NUnit.Framework;

using Foundation;

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

		[Test]
		public void TypeConverter_BuiltIn ()
		{
			Assert.NotNull (TypeDescriptor.GetConverter (new BuiltInConverter ()), "BuiltInConverter");

			string name = (typeof (BuiltInConverter).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
			Assert.That (name, Is.EqualTo ("System.ComponentModel.BooleanConverter, System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"), "ConverterTypeName");
		}

		[Test]
		public void TypeConverter_Custom ()
		{
			Assert.NotNull (TypeDescriptor.GetConverter (new TypeDescriptorTest ()), "TypeDescriptorTest");

			string name = (typeof (TypeDescriptorTest).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
			Assert.That (name, Is.EqualTo ("LinkAll.CustomConverter, link all, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"), "ConverterTypeName");
		}
	}
}