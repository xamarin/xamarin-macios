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
using System.Runtime.InteropServices;

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
				select m).FirstOrDefault();

			if (method != null) {
				var paramType = method.GetParameters () [0].ParameterType;
				if (paramType.Name == "String")
					return typeof (NSString);
				else
					return paramType;
			}
			else
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

		[DllImport (Constants.FoundationLibrary)]
		extern static IntPtr NSStringFromClass (IntPtr ptr);

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
				if (fi == null)
					continue;			
				IntPtr class_ptr = (IntPtr) fi.GetValue (null);
				IntPtr register_class_ptr = GetClassPtrFromRegister (t);

				///////
				var ret1 = (string) Runtime.GetNSObject<NSString> (NSStringFromClass (class_ptr));
				var ret2 = (string) Runtime.GetNSObject<NSString> (NSStringFromClass (register_class_ptr));
				var ret3 = Runtime.GetNSObject<NSString> (NSStringFromClass (class_ptr));
				///////
				///

				//Console.WriteLine ($"This is ret2: {ret2}");

				Assert.AreEqual (class_ptr, register_class_ptr, $"class_ptr and RegisterAttribute are different: t.Name: {t.Name} ret1: {ret1} ret2: {ret2} ret3: {ret3}");
			}
		}

		[Test]
		public void VerifyClassPtrCategories ()
		{
			foreach (Type t in Assembly.GetTypes().Where (t => t.IsClass && t.IsSealed && t.IsAbstract)) {
				if (Skip (t))
					continue;

				FieldInfo fi = t.GetField ("class_ptr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				if (fi == null)
					continue;
				IntPtr class_ptr = (IntPtr)fi.GetValue (null);

				var extendedType = GetExtendedType (t);
				IntPtr extended_class_ptr;
				if (extendedType == null)
					extended_class_ptr = IntPtr.Zero;
				else
					extended_class_ptr = GetClassPtrFromRegister (extendedType);

				Assert.AreEqual (class_ptr, extended_class_ptr, "class_ptr and RegisterAttribute from extended class are different: " + t.Name);
			}
		}
	}
}

