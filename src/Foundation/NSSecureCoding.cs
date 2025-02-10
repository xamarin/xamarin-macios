// NSSecureCoding support

using System;
using ObjCRuntime;

#nullable enable

namespace Foundation {

#if NET
	public static partial class NSSecureCoding {
#else
	public partial class NSSecureCoding {
#endif

		const string selConformsToProtocol = "conformsToProtocol:";
		const string selSupportsSecureCoding = "supportsSecureCoding";
#if !MONOMAC
		static IntPtr selConformsToProtocolHandle = Selector.GetHandle (selConformsToProtocol);
		static IntPtr selSupportsSecureCodingHandle = Selector.GetHandle (selSupportsSecureCoding);
#endif

		public static bool SupportsSecureCoding (Type type)
		{
			if (type is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (type));

#if MONOMAC
			try {
				return SupportsSecureCoding (new Class (type));
			}
			catch (ArgumentException) {
				// unlike XI the current registration will throw for protocols
				// until that's fixed we'll only report correctly properly bound protocol types
				// the workaround is important since this method is used to validate our bindings
				return typeof (INSSecureCoding).IsAssignableFrom (type);
			}
#else
			return SupportsSecureCoding (new Class (type));
#endif
		}

		public static bool SupportsSecureCoding (Class klass)
		{
			if (klass is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (klass));
			return SupportsSecureCoding (klass.Handle);
		}

		internal static bool SupportsSecureCoding (IntPtr ptr)
		{
			// iOS6+ and OSX 10.8+
			var secure_coding = Runtime.GetProtocol ("NSSecureCoding");
			if (secure_coding == IntPtr.Zero)
				return false;
#if MONOMAC
			if (Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("conformsToProtocol:"), secure_coding) == 0)
				return false;

			return Messaging.bool_objc_msgSend (ptr, Selector.GetHandle ("supportsSecureCoding")) != 0;
#else
			if (Messaging.bool_objc_msgSend_IntPtr (ptr, selConformsToProtocolHandle, secure_coding) == 0)
				return false;

			return Messaging.bool_objc_msgSend (ptr, selSupportsSecureCodingHandle) != 0;
#endif
		}
	}
}
