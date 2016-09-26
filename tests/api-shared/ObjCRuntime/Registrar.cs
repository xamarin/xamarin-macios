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

#if XAMCORE_2_0
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

		static FieldInfo GetPrivateField (Type type, string name)
		{
			var fld = type.GetField (name, BindingFlags.Instance | BindingFlags.NonPublic);
			if (fld != null)
				return fld;
			if (type.BaseType == null)
				return null;
			return GetPrivateField (type.BaseType, name);
		}

		public static Registrars CurrentRegistrar {
			get {
				var registrar = typeof(Runtime).GetField ("Registrar", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue (null);
				var dict = (System.Collections.IDictionary) GetPrivateField (registrar.GetType (), "lazy_map").GetValue (registrar);
				var is_static = dict.Contains (Class.GetHandle (typeof (RegistrationTestClass)));
				if (is_static) {
					return Registrars.Static;
				} else {
					return Registrars.Dynamic;
				}
			}
		}
	}
}
