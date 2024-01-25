using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

using MonoTouch.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	// Important: This is NOT a general purpose code remover. There are a lot of
	// cases to cover and this implementation cover only the one it requires so far.
	// IOW additional testing is REQUIRED if you change the scope of the code.
	public class RemoveRejectedTypesStep : RemoveCodeBase {

		public RemoveRejectedTypesStep ()
		{
		}

		public DerivedLinkContext Context {
			get { return context as DerivedLinkContext; }
		}

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly; }
		}

		protected override string Name { get; } = "Removing Rejected Type";
		protected override int ErrorCode { get; } = 2220;

		public List<(string originalFullName, string replacementTypeName)> TypeReferencesToBeRemoved = new List<(string, string)> () {
			// order is important for nested types (that's why tuples are used instead of a Dictionary)
			( "UIKit.UIWebView/_UIWebViewDelegate", "_DeprecatedWebViewDelegate"),
			( "UIKit.UIWebView/UIWebViewAppearance","DeprecatedWebViewAppearance"),
			( "UIKit.IUIWebViewDelegate","IDeprecatedWebViewDelegate"),
			( "UIKit.UIWebView","DeprecatedWebView"),
			( "UIKit.UIWebViewDelegate","DeprecatedWebViewDelegate"),
			( "UIKit.UIWebViewDelegate_Extensions", "DeprecatedWebViewDelegate_Extensions"),
			( "UIKit.UIWebViewDelegateWrapper", "DeprecatedWebViewDelegateWrapper"),
			( "UIKit.UIWebViewNavigationType", "DeprecatedWebViewNavigationType"),
		};

		protected override void Process (AssemblyDefinition assembly)
		{
			// this will give us a working `get_nse`
			base.Process (assembly);

			var updated = false;
			if (Profile.IsProductAssembly (assembly)) {
				updated = true;
				foreach (var item in TypeReferencesToBeRemoved) {
					var type = assembly.MainModule.GetType (item.originalFullName);
					if (type is null)
						continue;
					type.Name = item.replacementTypeName;
					Context.AddLinkedAwayType (type);
					// we need to remove [Register], [Protocol] and [ProtocolMembers] attributes
					// and since other attributes won't make sense anymore so we remove them all
					type.CustomAttributes.Clear ();
					if (!type.IsInterface) {
						// remove IL and custom attributes (e.g. [Export])
						ProcessMethods (type);
					}
				}
			} else if (Profile.IsSdkAssembly (assembly)) {
				// note: we know the BCL does not include references to UIWebView
				// so we only need to care for (i.e. tweak) user code
				return;
			} else {
				foreach (var module in assembly.Modules) {
					foreach (var item in TypeReferencesToBeRemoved) {
						if (module.TryGetTypeReference (item.originalFullName, out var tr)) {
							tr.Name = item.replacementTypeName;
							updated = true;
						}
					}
				}
			}
			// we'll need to save (if we're not linking) this assembly
			if (updated && Annotations.GetAction (assembly) != AssemblyAction.Link)
				Annotations.SetAction (assembly, AssemblyAction.Save);
		}

		protected override void ProcessMethod (MethodDefinition method)
		{
			if (method.HasCustomAttributes)
				method.CustomAttributes.Clear ();

			// single special case: deal with method `EnsureUIWebViewDelegate`
			if (method.Name == "EnsureUIWebViewDelegate")
				method.Name = "EnsureDeprecatedWebViewDelegate";

			base.ProcessMethod (method);
		}
	}
}
