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
using PlatformLinkContext = MonoTouch.Tuner.MonoTouchLinkContext;
#else
using PlatformException = Xamarin.Bundler.MonoMacException;
using PlatformLinkContext = MonoMac.Tuner.MonoMacLinkContext;
#endif

#if MONOTOUCH
namespace MonoTouch.Tuner {
#else 
namespace MonoMac.Tuner {
#endif
	static partial class Linker {

		static void Process (Pipeline pipeline, PlatformLinkContext context)
		{
			try {
				pipeline.Process (context);
			} catch (Exception e) {
				var pe = HandlePipelineProcessException (e);
				if (pe == e) // Throw original if possible to preserve stack trace
					throw;
				throw pe;
			}
		}

		static Exception HandlePipelineProcessException (Exception e)
		{
			switch (e) {
			case AssemblyResolutionException are:
				return new PlatformException (2002, true, are, are.Message);
			case AggregateException ae:
				return ae;
			case PlatformException pe:
				return pe;
			case MarkException me:
			{
				var re = me.InnerException as ResolutionException;
				if (re == null) {
					if (me.InnerException != null) {
						return ErrorHelper.CreateError (2102, me, mtouch.mtouchErrors.MT2102, me.Method.FullName, me.Method.Module, me.InnerException.Message);
					} else {
						return ErrorHelper.CreateError (2102, me, mtouch.mtouchErrors.MT2102_A, me.Method.FullName, me.Method.Module);
					}
				} else {
					TypeReference tr = (re.Member as TypeReference);
					IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
					return ErrorHelper.CreateError (2101, me, mtouch.mtouchErrors.MT2101, re.Member, me.Method.FullName, scope);
				}
			}
			case ResolutionException re:
			{
				TypeReference tr = (re.Member as TypeReference);
				IMetadataScope scope = tr == null ? re.Member.DeclaringType.Scope : tr.Scope;
				return new PlatformException (2002, true, re, "Failed to resolve \"{0}\" reference from \"{1}\"", re.Member, scope);
			}
			case XmlResolutionException ex:
				return new PlatformException (2017, true, ex, "Could not process XML description: {0}", ex?.InnerException?.Message ?? ex.Message);
			default:
				if (e.InnerException != null)
					return HandlePipelineProcessException (e.InnerException);

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
				return new PlatformException (2001, true, e, "Could not link assemblies. {0}", message);
			}
		}
	}
}
