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
	class PropertyArgumentSemanticVisitor : TestVistorBase
	{
		protected int GetArgumentSemantic (MethodInfo p)
		{
				if (p == null)
					return 0;

				System.Attribute attr = System.Attribute.GetCustomAttributes (p).FirstOrDefault (x => x.GetType().Name == "ExportAttribute");
				if (attr != null)
					return (int)attr.GetType().GetProperty("ArgumentSemantic").GetValue(attr, null);
				return 0;
		}

		void ReportError (string typeName, string propertyName, int managedSetSemantic, ObjCPropertySetterKind nativeSetSemantic)
		{
			Console.WriteLine ("Possibly Incorrect Managed ArgumentSemantic: {0} on {1} has {2} but native value is {3}", propertyName, typeName, managedSetSemantic, nativeSetSemantic);
		}

		public override void VisitObjCPropertyDecl (ObjCPropertyDecl decl)
		{
			if (currentInterfaceDecl == null || currentType == null)
				return;

			PropertyInfo managedProperty = FindProperty (decl);
			if (managedProperty == null)
				return;

			bool nativeHasSetter = decl.Setter != null;
			bool managedHasSetter = managedProperty.CanWrite;

			if (nativeHasSetter && managedHasSetter) {
				ObjCPropertySetterKind nativeSetSemantic = decl.SetterKind;

				// Read as int so we don't have to depend on XM assembly for type
				//	public enum ArgumentSemantic : int {
				//		None = -1,
				//		Assign = 0,
				//		Copy = 1,
				//		Retain = 2,
				//		Weak = 3,
				//		Strong = Retain,
				//		UnsafeUnretained = Assign,
				//	}
				int managedSetSemantic = GetArgumentSemantic (managedProperty.GetSetMethod ());

				// If our data type is string or string [], we _better_ be Copy on the native side.
				// Since we copy to/from .NET strings, ignore these to reduce noise
				if (nativeSetSemantic == ObjCPropertySetterKind.Copy &&
					(managedProperty.PropertyType == typeof (string) || managedProperty.PropertyType == typeof (string [])))
					return;

				switch (nativeSetSemantic)
				{
					case ObjCPropertySetterKind.Assign:
						if (managedSetSemantic != 0 && managedSetSemantic != -1) // None maps to Assign
							ReportError (currentType.Name, managedProperty.Name, managedSetSemantic, nativeSetSemantic);
						break;
					case ObjCPropertySetterKind.Retain:
						if (managedSetSemantic != 2)
							ReportError (currentType.Name, managedProperty.Name, managedSetSemantic, nativeSetSemantic);
						break;
					case ObjCPropertySetterKind.Copy:
						if (managedSetSemantic != 1)
							ReportError (currentType.Name, managedProperty.Name, managedSetSemantic, nativeSetSemantic);
						break;
					case ObjCPropertySetterKind.Weak:
						if (managedSetSemantic != 3)
							ReportError (currentType.Name, managedProperty.Name, managedSetSemantic, nativeSetSemantic);
						break;
					default:
						throw new InvalidOperationException (string.Format ("PropertyArgumentSemanticVisitor - Unknown semantic"));
				}
			}
		}
	}
}