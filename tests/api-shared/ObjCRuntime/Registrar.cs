//
// Unit tests for the registrars.
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#if __UNIFIED__
using Foundation;
using ObjCRuntime;
#elif __IOS__
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif

namespace XamarinTests.ObjCRuntime {

	public enum Registrars {
		Static = 1,
		Dynamic = 4,
		AllStatic = Static,
		AllDynamic = Dynamic,
	}
		
	public class Registrar {
		[Register ("__registration_test_CLASS")]
		class RegistrationTestClass : NSObject {}

		public static Registrars CurrentRegistrar {
			get {
				var find_type = typeof (Class).GetMethod ("FindType", BindingFlags.Static | BindingFlags.NonPublic);
				var type_to_find = typeof (RegistrationTestClass);
				var type = (Type) find_type.Invoke (null, new object [] { Class.GetHandle (type_to_find), false });
				var is_static = type_to_find == type;
				if (is_static) {
					return Registrars.Static;
				} else {
					return Registrars.Dynamic;
				}
			}
		}
	}
}
