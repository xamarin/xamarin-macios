// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.ComponentModel;
using MonoMac.AppKit;

// Test
// * Application use System.ComponentModel.TypeDescriptor
// * Application includes every *default* converter (see TypeDescriptor.cs)
// * Application does not include CustomConverter as it's not used by the application
//
// Requirement
// * Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	public class CustomConverter : TypeConverter {
	} 

	class TypeDescriptorTest {
		static string typeToCheck = "Xamarin.Mac.Linker.Test.CustomConverter";

		static void Check (string typeName)
		{
			var t = Type.GetType (typeName + ", System", false);
			Test.Log.WriteLine ("{0}\t{1}", t != null ? "[PASS]" : "[FAIL]", typeName);
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			TypeDescriptor td = null;
			Test.Log.WriteLine ("{0}\tTypeDescriptor", td == null ? "[PASS]" : "[FAIL]");

			Check ("System.ComponentModel.BooleanConverter");
			Check ("System.ComponentModel.ByteConverter");
			Check ("System.ComponentModel.SByteConverter");
			Check ("System.ComponentModel.StringConverter");
			Check ("System.ComponentModel.CharConverter");
			Check ("System.ComponentModel.Int16Converter");
			Check ("System.ComponentModel.Int32Converter");
			Check ("System.ComponentModel.Int64Converter");
			Check ("System.ComponentModel.UInt16Converter");
			Check ("System.ComponentModel.UInt32Converter");
			Check ("System.ComponentModel.UInt64Converter");
			Check ("System.ComponentModel.SingleConverter");
			Check ("System.ComponentModel.DoubleConverter");
			Check ("System.ComponentModel.DecimalConverter");
			Check ("System.ComponentModel.TypeConverter");
			Check ("System.ComponentModel.ArrayConverter");
			Check ("System.ComponentModel.CultureInfoConverter");
			Check ("System.ComponentModel.DateTimeConverter");
			Check ("System.ComponentModel.GuidConverter");
			Check ("System.ComponentModel.TimeSpanConverter");
			Check ("System.ComponentModel.CollectionConverter");
			Check ("System.ComponentModel.EnumConverter");

			var t = Type.GetType (typeToCheck, false);
			Test.Log.WriteLine ("{0}\tXamarin.Mac.Linker.Test.CustomConverter", t == null ? "[PASS]" : "[FAIL]");

			Test.Terminate ();
		}
	}
}