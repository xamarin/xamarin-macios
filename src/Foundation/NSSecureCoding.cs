// NSSecureCoding support

using System;
using ObjCRuntime;

namespace Foundation {

#if XAMCORE_4_0
	static
#endif
	public partial class NSSecureCoding {

		const string selConformsToProtocol = "conformsToProtocol:";
		const string selSupportsSecureCoding = "supportsSecureCoding";
#if !MONOMAC
		static IntPtr selConformsToProtocolHandle = Selector.GetHandle (selConformsToProtocol);
		static IntPtr selSupportsSecureCodingHandle = Selector.GetHandle (selSupportsSecureCoding);
#endif

		[iOS (6,0)]
		[Mac (10,8)]
		public static bool SupportsSecureCoding (Type type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

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

		[iOS (6,0)]
		[Mac (10,8)]
		public static bool SupportsSecureCoding (Class klass)
		{
			if (klass == null)
				throw new ArgumentNullException ("klass");
			return SupportsSecureCoding (klass.Handle);
		}

		internal static bool SupportsSecureCoding (IntPtr ptr)
		{
			// iOS6+ and OSX 10.8+
			var secure_coding = Runtime.GetProtocol ("NSSecureCoding");
			if (secure_coding == IntPtr.Zero)
				return false;
#if MONOMAC
			if (!Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("conformsToProtocol:"), secure_coding))
				return false;

			return Messaging.bool_objc_msgSend (ptr, Selector.GetHandle ("supportsSecureCoding"));
#else
			if (!Messaging.bool_objc_msgSend_IntPtr (ptr, selConformsToProtocolHandle, secure_coding))
				return false;

			return Messaging.bool_objc_msgSend (ptr, selSupportsSecureCodingHandle);
#endif
		}
	}
}