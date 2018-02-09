using System;
#if !__WATCHOS__
using System.Drawing;
#endif

#if __UNIFIED__
using ObjCRuntime;
using Foundation;
#if __MACOS__
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace Bindings.Test.Protocol {
	
	[Protocol]
	public interface P1 {
	}

	[Protocol, BaseType (typeof (NSObject))]
	public interface P2 {
	}

	[Protocol, Model, BaseType (typeof (NSObject))]
	public interface P3 {
	}

	[Protocol]
	public interface MemberAttributes {
		// Methods
		[Abstract] [Export ("requiredInstanceMethod")]
		void RequiredInstanceMethod ();

		[Export ("optionalInstanceMethod")]
		void OptionalInstanceMethod ();

		[Static] [Abstract] [Export ("requiredStaticMethod")]
		void RequiredStaticMethod ();

		[Static] [Export ("optionalStaticMethod")]
		void OptionalStaticMethod ();

		[Export ("variadicMethod:", IsVariadic = true)]
		void VariadicMethod (IntPtr list);

		[Export ("methodWithReturnType")]
		NSSet MethodWithReturnType ();

		[Export ("methodWithParameter:")]
		void MethodWithParameter (int p1);

		[Export ("methodWithParameters:second:third:fourth:")]
		void MethodWithParameters (int p1, int p2, int p3, int p4);

		[Export ("methodWithRefParameters:second:third:fourth:")]
		unsafe void MethodWithRefParameters (int p1, ref int p2, out int p3, int p4);

		// Properties
		[Abstract] [Export ("requiredInstanceProperty")]
		string RequiredInstanceProperty { get; set; }

		[Export ("optionalInstanceProperty")]
		string OptionalInstanceProperty { get; set; }

		[Static] [Abstract] [Export ("requiredStaticProperty")]
		string RequiredStaticProperty { get; set; }

		[Static] [Export ("optionalStaticProperty")]
		string OptionalStaticProperty { get; set; }

		[Export ("propertyWithCustomAccessors")]
		string PropertyWithCustomAccessors { [Bind ("get_propertyWithCustomAccessors")] get; [Bind ("set_propertyWithCustomAccessors:")] set; }

		[Export ("propertyWithArgumentSemanticNone", ArgumentSemantic = ArgumentSemantic.None)]
		string PropertyWithArgumentSemanticNone { get; set; }

		[Export ("propertyWithArgumentSemanticCopy", ArgumentSemantic = ArgumentSemantic.Copy)]
		string PropertyWithArgumentSemanticCopy { get; set; }

		[Export ("propertyWithArgumentSemanticAssign", ArgumentSemantic = ArgumentSemantic.Assign)]
		string PropertyWithArgumentSemanticAssign { get; set; }

		[Export ("propertyWithArgumentSemanticRetain", ArgumentSemantic = ArgumentSemantic.Retain)]
		string PropertyWithArgumentSemanticRetain { get; set; }

		[Export ("readonlyProperty")]
		string ReadonlyProperty { get; }

		[Abstract]
		[Export ("requiredReadonlyProperty")]
		NSString RequiredReadonlyProperty { get; }
	}
}
