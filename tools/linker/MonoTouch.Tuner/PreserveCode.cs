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
		}

		void PreserveDictionaryConstructor ()
		{
			var dictionary = Context.Corlib.MainModule.GetType ("System.Collections.Generic", "Dictionary`2");
			if (dictionary is null || !dictionary.HasMethods)
				return;

			foreach (MethodDefinition ctor in dictionary.Methods) {
				if (ctor.IsConstructor && ctor.HasParameters && ctor.Parameters [0].ParameterType.Is ("System", "Int32"))
					Context.Annotations.AddPreservedMethod (dictionary, ctor);
			}
		}

		void PreserveQueryableEnumerable ()
		{
			AssemblyDefinition core;
			if (!Context.TryGetLinkedAssembly ("System.Core", out core))
				return;

			var queryable_enumerable = core.MainModule.GetType ("System.Linq", "QueryableEnumerable`1");
			if (queryable_enumerable is null)
				return;

			var a = Context.Annotations;

			var enumerable = core.MainModule.GetType ("System.Linq", "Enumerable");
			foreach (MethodDefinition method in enumerable.Methods)
				a.AddPreservedMethod (queryable_enumerable, method);

			var queryable = core.MainModule.GetType ("System.Linq", "Queryable");
			foreach (MethodDefinition method in queryable.Methods)
				a.AddPreservedMethod (queryable_enumerable, method);
		}
	}
}
