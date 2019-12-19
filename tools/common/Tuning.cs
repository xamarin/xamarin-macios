using System;
using System.IO;
using System.Text;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Bundler;

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
			switch (e) {
			case AssemblyResolutionException are:
				throw new PlatformException (2002, true, are, are.Message);
			case AggregateException ae:
				throw ae;
			case PlatformException pe:
				throw pe;
			case MarkException me:
			{
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
			}
			case ResolutionException re:
			{
				TypeReference tr = (re.Member as TypeReference);
				IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
				throw new PlatformException (2002, true, re, "Failed to resolve \"{0}\" reference from \"{1}\"", re.Member, scope);
			}
			case XmlResolutionException ex:
				throw new PlatformException (2017, true, ex, "Could not process XML description: {0}", ex?.InnerException?.Message ?? ex.Message);
			default:
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
