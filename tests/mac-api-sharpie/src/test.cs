using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Clang;
using Clang.Ast;

using Sharpie;
using Sharpie.Tooling;
using Sharpie.Bind;
using Sharpie.Mono.CSharp;

namespace APITestTool
{
	[Tool ("APITest", "Test built Xamarin.Mac assembly against the headers", 500)]
	public class APITestTool : CommonTool
	{
		public static Assembly XamarinMacAssembly { get; private set; }

		protected override void Run ()
		{
			base.Run ();

			var pchFile = PositionalArguments.DequeueOrDefault ();
			string xmPath = PositionalArguments.DequeueOrDefault ();

			if (pchFile == null || !File.Exists (pchFile))
				throw new ExitException ("PCH file must be specified");

			if (xmPath == null || !File.Exists (xmPath))
				throw new ExitException ("Xamarin.Mac Assembly must be specified");

			XamarinMacAssembly = Assembly.LoadFrom (xmPath);
			if (XamarinMacAssembly == null)
				throw new ExitException ("Unable to load Xamarin.Mac Assembly");

			var reader = new AstReader ();
			reader.TranslationUnitParsed += tu => tu.Accept (new PropertyGetSetMismatchVisitor ());
			reader.TranslationUnitParsed += tu => tu.Accept (new PropertyArgumentSemanticVisitor ());

			if (!reader.Load (pchFile))
				throw new ExitException ("PCH file failed to load");
		}
	}

	class TestVistorBase : AstVisitor
	{
		protected ObjCInterfaceDecl currentInterfaceDecl;
		protected System.Type currentType;

		protected bool IsPrivate (string typeName)
		{
			// Let's skip anything that looks internal to test against
			return typeName.Contains ("Private") || typeName.Contains ("Internal") || typeName.StartsWith ("_");
		}

		public override void VisitObjCInterfaceDecl (ObjCInterfaceDecl decl, VisitKind visitKind)
		{
			if (visitKind == VisitKind.Enter && !IsPrivate (decl.Name)) {
				currentInterfaceDecl = decl;
				currentType = APITestTool.XamarinMacAssembly.GetTypes ().FirstOrDefault (x => x.Name == Extrospection.Helpers.GetManagedName(decl.Name)); // Technically SingleOrDefault, but significantly slower

				// If we couldn't match up a type, we're going to skip testing for now.
				if (currentType == null)
					currentInterfaceDecl = null;
			}
			else {
				currentInterfaceDecl = null;
				currentType = null;
			}
		}

		protected string GetSelector (MethodInfo p)
		{
				if (p == null)
					return null;

				System.Attribute attr = System.Attribute.GetCustomAttributes (p).FirstOrDefault (x => x.GetType().Name == "ExportAttribute");
				if (attr != null)
					return (string)attr.GetType().GetProperty("Selector").GetValue(attr, null);
				return null;
		}

		protected PropertyInfo FindProperty (ObjCPropertyDecl prop)
		{
			string getNativeSelector = prop.Getter != null ? prop.Getter.Name : null;
			string setNativeSelector = prop.Setter != null ? prop.Setter.Name : null;
			foreach (PropertyInfo p in currentType.GetProperties ())
			{
				string getManagedSelector = GetSelector (p.GetGetMethod ());
				string setManagedSelector = GetSelector (p.GetSetMethod ());
				if (getNativeSelector == getManagedSelector && setNativeSelector == setManagedSelector)
					return p;
			}
			return null;
		}
	}
}
