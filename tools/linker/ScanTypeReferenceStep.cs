using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Linker.Steps;
using Xamarin.Bundler;

#if MTOUCH
using ProductException = Xamarin.Bundler.MonoTouchException;
#else
using ProductException = Xamarin.Bundler.MonoMacException;
#endif

namespace Xamarin.Linker.Steps {

	abstract public class ScanTypeReferenceStep : BaseStep {
		protected readonly List<string> lookfor;

		protected ScanTypeReferenceStep (List<string> list)
		{
			lookfor = list;
		}

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			foreach (var module in assembly.Modules) {
				foreach (var name in lookfor) {
					if (module.HasTypeReference (name))
						Report (name, assembly);
				}
			}
		}

		protected abstract void Report (string typeName, AssemblyDefinition assembly);
	}

	public class PreLinkScanTypeReferenceStep : ScanTypeReferenceStep {

		public PreLinkScanTypeReferenceStep (List<string> list) : base (list)
		{
		}

		protected override void Report (string typeName, AssemblyDefinition assembly)
		{
			ErrorHelper.Show (new ProductException (1502, false, Errors.MX1502, typeName, assembly));
		}
	}

	public class PostLinkScanTypeReferenceStep : ScanTypeReferenceStep {

		public PostLinkScanTypeReferenceStep (List<string> list) : base (list)
		{
		}

		protected override void Report (string typeName, AssemblyDefinition assembly)
		{
			ErrorHelper.Show (new ProductException (1503, false, Errors.MX1503, typeName, assembly));
		}
	}
}
