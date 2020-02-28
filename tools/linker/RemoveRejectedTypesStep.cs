using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using MonoTouch.Tuner;
using Xamarin.Tuner;

namespace Xamarin.Linker {

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

		protected override string Name { get; } = " Removing Rejected Type";
		protected override int ErrorCode { get; } = 2660;

		public List<string> ReferencesToBeRemoved = new List<string> () {
			// order is important for nested types
			"UIKit.UIWebView/_UIWebViewDelegate",
			"UIKit.UIWebView/UIWebViewAppearance",
			"UIKit.IUIWebViewDelegate",
			"UIKit.UIWebView",
			"UIKit.UIWebViewDelegate",
			"UIKit.UIWebViewDelegate_Extensions",
			"UIKit.UIWebViewDelegateWrapper",
			"UIKit.UIWebViewNavigationType",
		};

		protected override void Process (AssemblyDefinition assembly)
		{
			// this will give us a working `get_nse`
			base.Process (assembly);

			var updated = false;
			if (Profile.IsProductAssembly (assembly)) {
				updated = true;
				foreach (var remove in ReferencesToBeRemoved) {
					var type = assembly.MainModule.GetType (remove);
					if (type == null)
						continue;
					type.Name = type.Name.Replace ("UI", "Deprecated");
					Context.AddLinkedAwayType (type);
					// remove [Register], [Protocol] and [ProtocolMembers] attributes
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
					foreach (var remove in ReferencesToBeRemoved) {
						if (module.TryGetTypeReference (remove, out var tr)) {
							tr.Name = tr.Name.Replace ("UI", "Deprecated");
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

            // don't break IDisposable
//			if (method.Name != "Dispose") {
				// deal with method `EnsureUIWebViewDelegate`
				method.Name = method.Name.Replace ("UIWebView", "Deprecated");

			    base.ProcessMethod (method);
//		    }
		}
	}
}
