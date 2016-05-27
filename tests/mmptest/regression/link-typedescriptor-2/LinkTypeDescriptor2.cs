// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.ComponentModel;
using MonoMac.AppKit;

// Test
// * Application use [TypeConverter] on a custom (non default) TypeConverter
// * Application includes CustomConverter as it is used by the application
//
// Requirement
// * Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	public class CustomConverter : TypeConverter {
		// note: the default ctor will be preserved by the linker because it's used in a [TypeConverter] attribute
	} 

	[TypeConverter (typeof (BooleanConverter))]
	public class BuiltInConverter {
	}

	[TypeConverter (typeof (CustomConverter))]
	class TypeDescriptorTest {

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			TypeConverter converter = null;
			string msg = "BooleanConverter";

			try {
				converter = TypeDescriptor.GetConverter (new BuiltInConverter ());
				msg = (typeof (BuiltInConverter).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
			}
			catch (Exception e) {
				msg = e.ToString ();
			}
			finally {
				Test.Log.WriteLine ("[{0}]\tTypeDescriptor.GetConverter BooleanConverter", converter != null ? "PASS" : "FAIL");
				bool success = msg == "System.ComponentModel.BooleanConverter, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
				Test.Log.WriteLine ("[{0}]\t{1}", success ? "PASS" : "FAIL", msg);
			}

			msg = "CustomConverter";
			try {
				converter = TypeDescriptor.GetConverter (new TypeDescriptorTest ());
				msg = (typeof (TypeDescriptorTest).GetCustomAttributes (false) [0] as TypeConverterAttribute).ConverterTypeName;
			}
			catch (Exception e) {
				msg = e.ToString ();
			}
			finally {
				Test.Log.WriteLine ("[{0}]\tTypeDescriptor.GetConverter CustomConverter", converter != null ? "PASS" : "FAIL");
				bool success = msg == "Xamarin.Mac.Linker.Test.CustomConverter, link-typedescriptor-2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
				Test.Log.WriteLine ("[{0}]\t{1}", success ? "PASS" : "FAIL", msg);
			}

			Test.Terminate ();
		}
	}
}