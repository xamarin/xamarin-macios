using System;
using Foundation;
#if IOS
using UIKit;
#endif

namespace XmlDocumentation {
	/// <summary>
	/// Summary for T1
	/// </summary>
	[BaseType (typeof (NSObject))]
	interface T1 : P1 {
		/// <summary>
		/// Summary for T2.#ctor(String)
		/// </summary>
		[Export ("initWithString:")]
		IntPtr Constructor (string p);

		/// <summary>
		/// Summary for T1.Method
		/// </summary>
		[Export ("method")]
		int Method ();

		/// <summary>
		/// Summary for T1.Property
		/// </summary>
		[Export ("property")]
		int Property { get; set; }

		// can't apply xml docs to a getter/setter, only the property itself
	}

#if IOS
	/// <summary>
	/// Summary for TypeWithApparance
	/// </summary>
	[BaseType (typeof (NSObject))]
	interface TypeWithApparance : IUIAppearance {
		/// <summary>
		/// Summary for TypeWithApparance.TintColor
		/// </summary>
		[Export ("tintColor")]
		[Appearance]
		UIColor TintColor { get; set; }
	}
#endif // IOS

	/// <summary>
	/// Summary for P1
	/// </summary>
	[Protocol]
	interface P1 {
		/// <summary>
		/// Summary for P1.PMethod
		/// </summary>
		[Export ("method")]
		int PMethod ();

		/// <summary>
		/// Summary for P1.PProperty
		/// </summary>
		[Export ("property")]
		int PProperty { get; set; }

		/// <summary>
		/// Summary for PA1.PMethod
		/// </summary>
		[Abstract]
		[Export ("methodRequired")]
		int PAMethod ();

		/// <summary>
		/// Summary for PA1.PProperty
		/// </summary>
		[Abstract]
		[Export ("propertyRequired")]
		int PAProperty { get; set; }
	}

	/// <summary>
	/// Summary for TG1
	/// </summary>
	[BaseType (typeof (NSObject))]
	interface TG1<T, U>
		where T : NSObject
		where U : NSObject {

		/// <summary>
		/// Summary for TG1.TGMethod
		/// </summary>
		[Export ("method")]
		int TGMethod ();

		/// <summary>
		/// Summary for TG1.TGProperty
		/// </summary>
		[Export ("property")]
		int TGProperty { get; set; }

		[Export ("method2:")]
		void TGMethod2 (TG1<T, U> value);
	}

	/// <summary>
	/// Summary for E1
	/// </summary>
	public enum E1 {
		/// <summary>
		/// Summary for E1.Value1
		/// </summary>
		Value1,
	}

	/// <summary>
	/// Summary for Notification1
	/// </summary>
	[BaseType (typeof (NSObject))]
	interface Notification1 {
		/// <summary>Summary for DocumentedNotification</summary>
		[Notification]
		[Field ("NSANotification", LibraryName = "__Internal")]
		NSString ANotification { get; }
	}

	/// <summary>
	/// Summary for E2
	/// </summary>
	enum E2 {
		/// <summary>
		/// Summary for E2.A
		/// </summary>
		[Field ("E2A", LibraryName = "__Internal")]
		A,
	}

	/// <summary>
	/// Summary for E3
	/// </summary>
	[ErrorDomain ("E3Domain", LibraryName = "__Internal")]
	enum E3 {
		/// <summary>
		/// Summary for E3.ErrorA
		/// </summary>
		ErrorA,
	}
	/// <summary>
	/// Summary for E4
	/// </summary>
	[Flags]
	enum E4 {
		/// <summary>
		/// Summary for E4.Bit1
		/// </summary>
		Bit1 = 1,
		/// <summary>
		/// Summary for E4.Bit3
		/// </summary>
		Bit3 = 4,
	}

	/// <summary>Summary for OptionKeys.</summary>
	[Static]
	interface OptionKeys {
	/// <summary>Summary for Option1Key.</summary>
		[Field ("Option1Key", "TheLibrary")]
		NSString Option1 { get; }
	}

	/// <summary>Summary for OptionsA.</summary>
	[StrongDictionary ("OptionKeys")]
	interface OptionsA {
		/// <summary>Summary for Option1.</summary>
		[Export ("Option1")]
		string Option1 { get; set; }
	}

	// no summary here
	[StrongDictionary ("OptionKeys")]
	interface OptionsB {
		/// <summary>Summary for Option1.</summary>
		[Export ("Option1")]
		string Option1 { get; set; }
	}
}
