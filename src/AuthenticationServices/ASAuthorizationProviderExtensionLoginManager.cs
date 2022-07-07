//
// ASAuthorizationProviderExtensionLoginManager.cs
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Microsoft Co. All rights reserved.
//

#nullable enable

using System;

namespace AuthenticationServices {
	public partial class ASAuthorizationProviderExtensionLoginManager {

		// // -(void)saveCertificate:(SecCertificateRef _Nonnull)certificate keyType:(ASAuthorizationProviderExtensionKeyType)keyType __attribute__((swift_name("saveCertificate(_:keyType:)")));
		// [Export ("saveCertificate:keyType:")]
		// unsafe void SaveCertificate (SecCertificateRef* certificate, ASAuthorizationProviderExtensionKeyType keyType);

		public void SaveCertificate (SecCertificateRef* certificate, ASAuthorizationProviderExtensionKeyType keyType){

		}


		// [Export ("copyKeyForKeyType:")]
		// [return: NullAllowed]
		// unsafe SecKeyRef* CopyKey (ASAuthorizationProviderExtensionKeyType keyType);
		public SecKeyRef* CopyKey (ASAuthorizationProviderExtensionKeyType keyType) {

		}


		// public virtual CGPoint []? NormalizedPoints {
		// 	get {
		// 		var ret = _GetNormalizedPoints ();
		// 		if (ret == IntPtr.Zero)
		// 			return null;

		// 		unsafe {
		// 			var count = (int) PointCount;
		// 			var rv = new CGPoint [count];
		// 			var ptr = (CGPoint*) ret;
		// 			for (int i = 0; i < count; i++)
		// 				rv [i] = *ptr++;
		// 			return rv;
		// 		}
		// 	}
		// }

		// public virtual CGPoint []? GetPointsInImage (CGSize imageSize)
		// {
		// 	// return the address of the array of pointCount points
		// 	// or NULL if the conversion could not take place.
		// 	var ret = _GetPointsInImage (imageSize);
		// 	if (ret == IntPtr.Zero)
		// 		return null;

		// 	unsafe {
		// 		var count = (int) PointCount;
		// 		var rv = new CGPoint [count];
		// 		var ptr = (CGPoint*) ret;
		// 		for (int i = 0; i < count; i++)
		// 			rv [i] = *ptr++;
		// 		return rv;
		// 	}
		// }
	}
}
