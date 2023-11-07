using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Bundler;

#if MONOTOUCH
using PlatformLinkContext = MonoTouch.Tuner.MonoTouchLinkContext;
#else
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

		static void Append (this Pipeline self, IStep step)
		{
			self.AppendStep (step);
			if (Driver.WatchLevel > 0)
				self.AppendStep (new TimeStampStep (step));
		}

		static Exception HandlePipelineProcessException (Exception e)
		{
			switch (e) {
			case AssemblyResolutionException are:
				return new ProductException (2002, true, are, are.Message);
			case AggregateException ae:
				return ae;
			case ProductException pe:
				return pe;
			case MarkException me: {
				var re = me.InnerException as ResolutionException;
				if (re is null) {
					if (me.InnerException is not null) {
						return ErrorHelper.CreateError (2102, me, Errors.MT2102, me.Method.FullName, me.Method.Module, me.InnerException.Message);
					} else {
						return ErrorHelper.CreateError (2102, me, Errors.MT2102_A, me.Method.FullName, me.Method.Module);
					}
				} else {
					TypeReference tr = (re.Member as TypeReference);
					IMetadataScope scope = tr is null ? re.Member.DeclaringType.Scope : tr.Scope;
					return ErrorHelper.CreateError (2101, me, Errors.MT2101, re.Member, me.Method.FullName, scope);
				}
			}
			case ResolutionException re: {
				TypeReference tr = (re.Member as TypeReference);
				IMetadataScope scope = tr is null ? re.Member.DeclaringType.Scope : tr.Scope;
				return new ProductException (2002, true, re, "Failed to resolve \"{0}\" reference from \"{1}\"", re.Member, scope);
			}
			case XmlResolutionException ex:
				return new ProductException (2017, true, ex, Errors.MX2017, ex?.InnerException?.Message ?? ex.Message);
			default:
				if (e.InnerException is not null)
					return HandlePipelineProcessException (e.InnerException);

				var message = new StringBuilder ();
				if (e.Data.Count > 0) {
					message.AppendLine ();
					var m = e.Data ["MethodDefinition"] as string;
					if (m is not null)
						message.AppendLine ($"\tMethod: `{m}`");
					var t = e.Data ["TypeReference"] as string;
					if (t is not null)
						message.AppendLine ($"\tType: `{t}`");
					var a = e.Data ["AssemblyDefinition"] as string;
					if (a is not null)
						message.AppendLine ($"\tAssembly: `{a}`");
				}
				message.Append ($"Reason: {e.Message}");
				return new ProductException (2001, true, e, Errors.MX2001, message);
			}
		}
	}

	public class TimeStampStep : IStep {
		string message;

		public TimeStampStep (IStep step)
		{
			message = step.ToString ();
		}

		public void Process (LinkContext context)
		{
			Driver.Watch (message, 2);
		}
	}

	public class CustomizeCoreActions : CustomizeActions {

		public CustomizeCoreActions (bool link_sdk_only, IEnumerable<string> skipped_assemblies)
			: base (link_sdk_only, skipped_assemblies)
		{
		}

		protected override bool IsLinkerSafeAttribute (CustomAttribute attribute)
		{
			var at = attribute.AttributeType;
			switch (at.Name) {
			case "LinkerSafeAttribute":
				return true; // namespace is not important
			case "AssemblyMetadataAttribute":
				// this is only true for net6+ and can depends on other features not available on the legacy linker
				return false;
			}
			return false;
		}
	}
}
