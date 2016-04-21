using System;
using System.Reflection;

namespace XamCore.ObjCRuntime {
#if XAMCORE_2_0
	internal
#else
	public
#endif
	struct MethodDescription {
#if XAMCORE_2_0 || MONOTOUCH
		internal
#else
		public
#endif
		MethodBase method;

#if XAMCORE_2_0 || MONOTOUCH
		internal
#else
		public
#endif
		ArgumentSemantic semantic;

#if !COREBUILD

		// The ArgumentSemantic enum is public, and
		// I don't want to add another enum value there which
		// is just an internal implementation detail, so just
		// use a constant instead. Eventually we'll use an internal
		// enum instead.
		const int RetainReturnValueFlag = 1 << 10;
		const int InstanceCategoryFlag = 1 << 11;

		internal bool IsInstanceCategory {
			get { return (semantic & (ArgumentSemantic) InstanceCategoryFlag) == (ArgumentSemantic) InstanceCategoryFlag; }
		}
	
		public MethodDescription (MethodBase method, ArgumentSemantic semantic) {
			var minfo = method as MethodInfo;
			var retainReturnValue = minfo != null && minfo.GetBaseDefinition ().ReturnTypeCustomAttributes.IsDefined (typeof (ReleaseAttribute), false);
			var instanceCategory = minfo != null && XamCore.Registrar.DynamicRegistrar.HasThisAttributeImpl (minfo);

			// bitfields and a default value of -1 don't go very well together.
			if (semantic == ArgumentSemantic.None)
				semantic = ArgumentSemantic.Assign;

			if (retainReturnValue)
				semantic = semantic | (ArgumentSemantic) (RetainReturnValueFlag);
			if (instanceCategory)
				semantic |= (ArgumentSemantic) (InstanceCategoryFlag);

			this.method = method;
			this.semantic = semantic;
		}

		internal UnmanagedMethodDescription GetUnmanagedDescription () {
			return new UnmanagedMethodDescription (ObjectWrapper.Convert (method), semantic);
		}
#endif // !COREBUILD
	}

	internal struct UnmanagedMethodDescription {
		public IntPtr method;
		public ArgumentSemantic semantic;

		public UnmanagedMethodDescription (IntPtr method, ArgumentSemantic semantic) {
			this.method = method;
			this.semantic = semantic;
		}
	}
}
