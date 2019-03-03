// HttpTextWriter.cs: Class to report test results using http requests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc.
//

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if __UNIFIED__
using Foundation;
#else
using MonoTouch.Foundation;
#endif

namespace  BCLTests.TestRunner.Core {
	class HttpTextWriter : TextWriter
	{
		public string HostName;
		public int Port;

		TaskCompletionSource<bool> finished = new TaskCompletionSource<bool> ();
		TaskCompletionSource<bool> closed = new TaskCompletionSource<bool> ();
		StringBuilder log = new StringBuilder ();

		public Task FinishedTask {
			get {
				return finished.Task;
			}
		}

		public override Encoding Encoding {
			get {
				return Encoding.UTF8;
			}
		}

		public override void Close ()
		{
			var valueWasSet = closed.TrySetResult (true);
			if (valueWasSet) {
				Task.Run (async () =>
				{
					await finished.Task;
					base.Close ();
				});
			}
		}


		Task<bool> SendData (NSUrl url, string uploadData)
		{
			var tcs = new TaskCompletionSource<bool> ();
			var request = new NSMutableUrlRequest (url);
			request.HttpMethod = "POST";
			var rv = NSUrlSession.SharedSession.CreateUploadTask (request, NSData.FromString (uploadData), (NSData data, NSUrlResponse response, NSError error) =>
			{
				if (error != null) {
					Console.WriteLine ("Failed to send data to {0}: {1}", url.AbsoluteString, error);
					tcs.SetResult (false);
				} else {
					//Console.WriteLine ("Succeeded sending data to {0}", url.AbsoluteString);
					tcs.SetResult (true);
				}
			});
			rv.Resume ();
			return tcs.Task;
		}

		async Task SendData (string action, string uploadData)
		{
			var url = NSUrl.FromString ("http://" + HostName + ":" + Port + "/" + action);

			int attempts_left = 10;
			while (!await SendData (url, uploadData)) {
				if (--attempts_left == 0) {
					Console.WriteLine ("Not resending data anymore.");
					throw new Exception ("Failed to send data.");
				}
				Console.WriteLine ("Resending data: {0} Length: {1} to: {2} Attempts left: {3}", action, uploadData.Length, url.AbsoluteString, attempts_left);
			};
		}

		async void SendThread ()
		{
			try {
				await SendData ("Start", "");
				await closed.Task;
				await SendData ("Finish", log.ToString ());
			} catch (Exception ex) {
				Console.WriteLine ("HttpTextWriter failed: {0}", ex);
			} finally {
				finished.SetResult (true);				
			}
		}

		public void Open ()
		{
			new Thread (SendThread)
			{
				IsBackground = true,
			}.Start ();
		}

		public override void Write (char value)
		{
			Console.Out.Write (value);
			log.Append (value);
		}

		public override void Write (char [] buffer)
		{
			Console.Out.Write (buffer);
			log.Append (buffer);
		}
	
		public override void WriteLine (string value)
		{
			Console.Out.WriteLine (value);
			log.AppendLine (value);
		}
	}
}
