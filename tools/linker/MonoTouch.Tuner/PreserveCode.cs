using System;
using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class PreserveCode : CorePreserveCode {

		public PreserveCode (LinkerOptions options) : base (options.I18nAssemblies)
		{
			Device = options.Device;
		}

		public bool Device { get; set; }

		protected string ProductAssembly { get; private set; }

		public override void Process (LinkContext context)
		{
			ProductAssembly = (Profile.Current as BaseProfile).ProductAssembly;

			base.Process (context);

			PreserveDictionaryConstructor ();
			PreserveQueryableEnumerable ();

			if (context.GetParameter ("debug-build") == "True")
				PreserveDebugFeatures ();
		}

		void PreserveDictionaryConstructor ()
		{
			var dictionary = Corlib.MainModule.GetType ("System.Collections.Generic", "Dictionary`2");
			if (dictionary == null || !dictionary.HasMethods)
				return;

			foreach (MethodDefinition ctor in dictionary.Methods) {
				if (ctor.IsConstructor && ctor.HasParameters && ctor.Parameters [0].ParameterType.Is ("System", "Int32"))
					Context.Annotations.AddPreservedMethod (dictionary, ctor);
			}
		}

		void PreserveCalendar (string name)
		{
			var calendar = Corlib.MainModule.GetType ("System.Globalization", name);
			if (calendar == null || !calendar.HasMethods)
				return;

			// we just preserve the default .ctor so Activation.Create will work, 
			// the normal linker logic will do the rest
			foreach (MethodDefinition ctor in calendar.Methods) {
				if (ctor.IsConstructor && !ctor.IsStatic && !ctor.HasParameters) {
					Context.Annotations.AddPreservedMethod (calendar, ctor);
					// we need to mark the type or the above won't be processed
					Context.Annotations.Mark (calendar);
				}
			}
		}

		void PreserveQueryableEnumerable ()
		{
			AssemblyDefinition core;
			if (!Context.TryGetLinkedAssembly ("System.Core", out core))
				return;

			var queryable_enumerable = core.MainModule.GetType ("System.Linq", "QueryableEnumerable`1");
			if (queryable_enumerable == null)
				return;

			var a = Context.Annotations;

			var enumerable = core.MainModule.GetType ("System.Linq", "Enumerable");
			foreach (MethodDefinition method in enumerable.Methods)
				a.AddPreservedMethod (queryable_enumerable, method);

			var queryable = core.MainModule.GetType ("System.Linq", "Queryable");
			foreach (MethodDefinition method in queryable.Methods)
				a.AddPreservedMethod (queryable_enumerable, method);
		}

		void PreserveDebugFeatures ()
		{
			AssemblyDefinition monotouch;
			if (!Context.TryGetLinkedAssembly ((Profile.Current as BaseProfile).ProductAssembly, out monotouch))
				return;

			var klass = monotouch.MainModule.GetType (Namespaces.ObjCRuntime, "Class");
			if (klass == null || !klass.HasMethods)
				return;

			foreach (MethodDefinition method in klass.Methods) {
				if (method.Name == "LookupFullName")
					Context.Annotations.AddPreservedMethod (klass, method);
			}
		}

		// FIXME: for compatibility with the existing .xml files - it's not clear why they are needed
		// and git's history is not helpful. To be reviewed someday
		void PreserveOpenTk ()
		{
			AssemblyDefinition opentk;
			if (Context.TryGetLinkedAssembly ("OpenTK", out opentk))
				PreserveOpenTk (opentk);

			if (Context.TryGetLinkedAssembly ("OpenTK-1.0", out opentk))
				PreserveOpenTk (opentk);
		}

		void PreserveType (TypeDefinition type)
		{
			if (type == null)
				return;

			Context.Annotations.Mark (type);
			foreach (var method in type.Methods)
				Context.Annotations.AddPreservedMethod (type, method);

			if (!type.HasNestedTypes)
				return;

			foreach (var nested in type.NestedTypes)
				PreserveType (nested);
		}

		void PreserveOpenTk (AssemblyDefinition opentk)
		{
			// if OpenTK.dll or OpenTK-1.0.dll are used then the following types will always be preserved
			// that include the nested Core type (note: Delegates nested type does not exists ?anymore?)
			PreserveType (opentk.MainModule.GetType ("OpenTK.Graphics.ES11", "GL"));
			PreserveType (opentk.MainModule.GetType ("OpenTK.Graphics.ES20", "GL"));
		}
	}
}