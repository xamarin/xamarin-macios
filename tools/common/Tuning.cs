using System;
using System.IO;
using System.Text;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Bundler;


using Mono.Cecil;
using Mono.Cecil.Cil;


#if MONOTOUCH
using PlatformException = Xamarin.Bundler.MonoTouchException;
#else
using PlatformException = Xamarin.Bundler.MonoMacException;
#endif

#if MONOTOUCH
namespace MonoTouch.Tuner {
#else 
namespace MonoMac.Tuner {
#endif
	static partial class Linker {
		static void HandlePipelineProcessException (Exception e)
		{
			if (e is FileNotFoundException fnfe) {
				// Cecil throw this if the assembly is not found
				throw new PlatformException (2002, true, fnfe, fnfe.Message);
			} 
			else if (e is AssemblyResolutionException are) {
				throw new PlatformException (2002, true, are, are.Message);
			}
			else if (e is AggregateException) {
				throw e;
			} else if (e is PlatformException) {
				throw e;
			} else if (e is MarkException me) {
				var re = me.InnerException as ResolutionException;
				if (re == null) {
					if (me.InnerException != null) {
						throw ErrorHelper.CreateError (2102, me, "Error processing the method '{0}' in the assembly '{1}': {2}", me.Method.FullName, me.Method.Module, me.InnerException.Message);
					} else {
						throw ErrorHelper.CreateError (2102, me, "Error processing the method '{0}' in the assembly '{1}'", me.Method.FullName, me.Method.Module);
					}
				} else {
					TypeReference tr = (re.Member as TypeReference);
					IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
					throw ErrorHelper.CreateError (2101, me, "Can't resolve the reference '{0}', referenced from the method '{1}' in '{2}'.", re.Member, me.Method.FullName, scope);
				}
			} else if (e is ResolutionException re) {
				TypeReference tr = (re.Member as TypeReference);
				IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
				throw new PlatformException (2002, true, re, "Failed to resolve \"{0}\" reference from \"{1}\"", re.Member, scope);
			} else if (e is XmlResolutionException ex) {
				throw new PlatformException (2017, true, ex, "Could not process XML description: {0}", ex?.InnerException?.Message ?? ex.Message);
			} else {
				if (e.InnerException != null) {
					HandlePipelineProcessException (e.InnerException);
					return;
				}

				var message = new StringBuilder ();
				if (e.Data.Count > 0) {
					message.AppendLine ();
					var m = e.Data ["MethodDefinition"] as string;
					if (m != null)
						message.AppendLine ($"\tMethod: `{m}`");
					var t = e.Data ["TypeReference"] as string;
					if (t != null)
						message.AppendLine ($"\tType: `{t}`");
					var a = e.Data ["AssemblyDefinition"] as string;
					if (a != null)
						message.AppendLine ($"\tAssembly: `{a}`");
				}
				message.Append ($"Reason: {e.Message}");
				throw new PlatformException (2001, true, e, "Could not link assemblies. {0}", message);
			}
		}
	}
}
