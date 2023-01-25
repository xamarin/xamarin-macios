using System;
using Foundation;
using ObjCRuntime;

namespace ImageCaptureCore {
	partial class ICCameraDevice {

		public delegate void DidReadDataDelegate (NSData data, ICCameraFile file, NSError error);
		
		public void RequestReadDataFromFile (ICCameraFile file, long offset, long length, DidReadDataDelegate callback)
		{
			var actionObject = new DidReadDataFromFileAction (callback);
			RequestReadDataFromFile (file, offset, length, actionObject, Selector.FromHandle(Selector.GetHandle("didReadData:fromFile:error:contextInfo:")), IntPtr.Zero);
		}

		class DidReadDataFromFileAction : NSObject {
			DidReadDataDelegate Callback;
			public const string CallbackSelector = "didReadData:fromFile:error:contextInfo:";

			public DidReadDataFromFileAction (DidReadDataDelegate callback)
			{
				Callback = callback;
				IsDirectBinding = false;
			}

			[Export (CallbackSelector)]
			void DidReadDataDelegate (NSData data, ICCameraFile file, NSError error, IntPtr contextInfo)
			{
				Callback (data, file, error);
			}
		}

		public delegate void DidDownloadDataDelegate (ICCameraFile file, NSError error, NSDictionary<NSString, NSObject> options);
		
		public void RequestDownloadFile (ICCameraFile file, NSDictionary<NSString, NSObject> options, DidDownloadDataDelegate callback)
		{
			var actionObject = new DidDownloadDataFromFileAction (callback);
			RequestDownloadFile (file, options, actionObject, Selector.FromHandle(Selector.GetHandle("didDownloadFile:error:options:contextInfo:")), IntPtr.Zero);
		}
		
		class DidDownloadDataFromFileAction : ICCameraDeviceDownloadDelegate {
			DidDownloadDataDelegate Callback;
			public const string CallbackSelector = "didDownloadFile:error:options:contextInfo:";

			public DidDownloadDataFromFileAction (DidDownloadDataDelegate callback)
			{
				Callback = callback;
				IsDirectBinding = false;
			}

			[Export (CallbackSelector)]
			void DidDownloadDataDelegate (ICCameraFile file, NSError error, NSDictionary<NSString, NSObject> options, IntPtr contextInfo)
			{
				Callback (file, error, options);
			}
		}

		public delegate void DidSendPTPDelegate (NSData command, NSData data, NSData response, ICCameraFile file, NSError error);
		
		public void RequestSendPtpCommand (NSData command, NSData data, DidSendPTPDelegate callback)
		{
			var actionObject = new DidSendPTPAction (callback);
			RequestSendPtpCommand (command, data, actionObject, Selector.FromHandle(Selector.GetHandle("didSendPTPCommand:inData:response:error:contextInfo:")), IntPtr.Zero);
		}
		
		class DidSendPTPAction : NSObject {
			DidSendPTPDelegate Callback;
			public const string CallbackSelector = "didSendPTPCommand:inData:response:error:contextInfo:";

			public DidSendPTPAction (DidSendPTPDelegate callback)
			{
				Callback = callback;
				IsDirectBinding = false;
			}

			[Export (CallbackSelector)]
			void DidSendPTPDelegate (NSData command, NSData data, NSData response, ICCameraFile file, NSError error, IntPtr contextInfo)
			{
				Callback (command, data, response, file, error);
			}
		}

	}

}
