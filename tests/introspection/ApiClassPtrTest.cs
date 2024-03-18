//
// Test fixture for class_ptr introspection tests
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2012-2014, 2016 Xamarin Inc.
//
using System;
using System.Reflection;
using System.Linq;

using NUnit.Framework;
using Xamarin.Utils;
using System.Runtime.CompilerServices;

using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Introspection {

	public abstract class ApiClassPtrTest : ApiBaseTest {

		protected virtual bool Skip (Type type)
		{
			// skip delegate (and other protocol references)
			foreach (object ca in type.GetCustomAttributes (false)) {
				if (ca is ProtocolAttribute)
					return true;
				if (ca is ModelAttribute)
					return true;
			}

			// skip types that we renamed / rewrite since they won't behave correctly (by design)
			if (SkipDueToRejectedTypes (type))
				return true;

			return SkipDueToAttribute (type);
		}

		Type GetExtendedType (Type extensionType)
		{
			var method =
				(from m in extensionType.GetMethods (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				 where m.IsDefined (typeof (ExtensionAttribute), false)
				 select m).FirstOrDefault ();

			if (method is not null) {
				var paramType = method.GetParameters () [0].ParameterType;
				if (paramType.Name == "String")
					return typeof (NSString);
				else
					return paramType;
			} else
				return null;
		}

		IntPtr GetClassPtrFromRegister (Type t)
		{
			var attribs = t.GetCustomAttributes (typeof (RegisterAttribute), true);
			if (attribs.Length > 0) {
				var register = ((RegisterAttribute) attribs [0]);
				return Class.GetHandle (register.Name);
			}
			return IntPtr.Zero;
		}

		[Test]
		public void VerifyClassPtr ()
		{
			foreach (Type t in Assembly.GetTypes ()) {
				if (t.IsNested || !NSObjectType.IsAssignableFrom (t))
					continue;

				if (t.ContainsGenericParameters)
					continue;

				if (Skip (t))
					continue;

				FieldInfo fi = t.GetField ("class_ptr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				if (fi is null)
					continue;
				IntPtr class_ptr = (IntPtr) (NativeHandle) fi.GetValue (null);
				IntPtr register_class_ptr = GetClassPtrFromRegister (t);

				Assert.AreEqual (class_ptr, register_class_ptr, "class_ptr and RegisterAttribute are different: " + t.Name);
			}
		}

		[Test]
		public void VerifyClassPtrCategories ()
		{
			foreach (Type t in Assembly.GetTypes ().Where (t => t.IsClass && t.IsSealed && t.IsAbstract)) {
				if (Skip (t))
					continue;

				FieldInfo fi = t.GetField ("class_ptr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				if (fi is null)
					continue;
				IntPtr class_ptr = (IntPtr) (NativeHandle) fi.GetValue (null);

				var extendedType = GetExtendedType (t);
				IntPtr extended_class_ptr;
				if (extendedType is null)
					extended_class_ptr = IntPtr.Zero;
				else
					extended_class_ptr = GetClassPtrFromRegister (extendedType);

				Assert.AreEqual (class_ptr, extended_class_ptr, "class_ptr and RegisterAttribute from extended class are different: " + t.Name);
			}
		}
	}
}
