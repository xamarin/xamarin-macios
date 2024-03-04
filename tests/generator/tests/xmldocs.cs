using System;
using Foundation;

namespace XmlDocumentation {
	/// <summary>
	/// Summary for T1
	/// </summary>
	[BaseType (typeof (NSObject))]
	interface T1 : P1 {
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
		where T: NSObject
		where U: NSObject {

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
}
