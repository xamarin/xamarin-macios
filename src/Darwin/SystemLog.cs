//
// SystemLog.cs: Bindings to the asl(3) API from Apple
//
// Authors:
//  Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012, Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if MONOMAC

using System;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Darwin {

	public class SystemLog : IDisposable, INativeObject {
		static SystemLog _default;
		
		public static SystemLog Default {
			get {
				if (_default == null)
					_default = new SystemLog (IntPtr.Zero);
				return _default;
			}
		}
		
		[Flags]
		public enum Option { Stderr, NoDelay, NoRemote }

		bool disposed;
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~SystemLog ()
		{
			Dispose (false);
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (!disposed){
				asl_close (handle);
				disposed = true;
				handle = IntPtr.Zero;
			}
		}
		
		[DllImport (Constants.SystemLibrary)]
		extern static void asl_close (IntPtr handle);
		
		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_open (string ident, string facility, Option /* uint32_t */ options);

		SystemLog (IntPtr handle)
		{
			this.handle = handle;
		}
		
		public SystemLog (string ident, string facility, Option options = 0)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			if (facility == null)
				throw new ArgumentNullException ("facility");
			
			handle = asl_open (ident, facility, options);
		}

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_open_from_file (int /* int */ fd, string ident, string facility);
		
		public SystemLog (int fileDescriptor, string ident, string facility)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			if (facility == null)
				throw new ArgumentNullException ("facility");

			handle = asl_open_from_file (fileDescriptor, ident, facility);
		}

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_add_log_file (IntPtr handle, int /* int */ fd);

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_remove_log_file (IntPtr handle, int /* int */ fd);
		
		public void AddLogFile (int descriptor)
		{
			asl_add_log_file (handle, descriptor);
		}

		public void RemoveLogFile (int descriptor)
		{
			asl_remove_log_file (handle, descriptor);
		}

		[DllImport (Constants.SystemLibrary)]
		extern static int asl_log (IntPtr handle, IntPtr msgHandle, string text);

		public int Log (Message msg, string text, params object [] args)
		{
			var txt = text == null ? "" : String.Format (text, args);
			if (txt.IndexOf ('%') != -1)
				txt = txt.Replace ("%", "%%");
			return asl_log (handle, msg == null ? IntPtr.Zero : msg.Handle, txt);
		}

		public int Log (string text)
		{
			if (text == null)
				throw new ArgumentNullException ("text");
			
			return asl_log (handle, IntPtr.Zero, text);
		}
		
		[DllImport (Constants.SystemLibrary)]
		extern static int asl_send (IntPtr handle, IntPtr msgHandle);
		
		public int Log (Message msg)
		{
			if (msg == null)
				throw new ArgumentNullException ("msg");
			
			return asl_send (handle, msg.Handle);
		}

		[DllImport (Constants.SystemLibrary)]
		extern static int asl_set_filter (IntPtr handle, int /* int */ f);
		
		public int SetFilter (int level)
		{
			return asl_set_filter (handle, level);
		}

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_search (IntPtr handle, IntPtr msg);

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr aslresponse_next (IntPtr handle);
		
		[DllImport (Constants.SystemLibrary)]
		extern static void aslresponse_free (IntPtr handle);
		
		public IEnumerable<Message> Search (Message msg)
		{
			if (msg == null)
				throw new ArgumentNullException ("msg");
			var search = asl_search (handle, msg.Handle);
			IntPtr mh;
			
			while ((mh = aslresponse_next (search)) != IntPtr.Zero)
				yield return new Message (mh);

			aslresponse_free (search);
		}
	}

	public class Message : IDisposable, INativeObject {
		public IntPtr Handle { get; private set; }
		public enum Kind { Message, Query }

		[Flags]
		public enum Op {
			CaseFold = 0x10,
			Prefix = 0x20,
			Suffix = 0x40,
			Substring = 0x60,
			Numeric = 0x80,
				Regex = 0x100,
			Equal = 1,
			Greater = 2,
			GreaterEqual = 3,
			Less = 4,
			LessEqual = 5,
			NotEqual = 6,
			True = 7
		}

		internal Message (IntPtr handle)
		{
			this.Handle = handle;
		}
		
		public Message (Kind kind)
		{
			Handle = asl_new (kind);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~Message ()
		{
			Dispose (false);
		}
		
		[DllImport (Constants.SystemLibrary)]
		extern static void asl_free (IntPtr handle);

		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero){
				asl_free (Handle);
				Handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr asl_new (Kind kind);

		[DllImport (Constants.SystemLibrary)]
		extern static string asl_get (IntPtr handle, string key);
		
		[DllImport (Constants.SystemLibrary)]
		extern static int asl_set (IntPtr handle, string key, string value);

		public string this [string key] {
			get {
				if (key == null)
					throw new ArgumentNullException ("key");
				return asl_get (Handle, key);
			}
			set {
				if (key == null)
					throw new ArgumentNullException ("key");
				asl_set (Handle, key, value);
			}
		}

		[DllImport (Constants.SystemLibrary)]
		extern static int asl_unset (IntPtr handle, string key);
		
		public void Remove (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			asl_unset (Handle, key);
		}
		
		[DllImport (Constants.SystemLibrary)]
		extern static string asl_key (IntPtr handle, int /* uint32_t */ key);
		
		public string this [int key]{
			get {
				return asl_key (Handle, key);
			}
		}

		public string Time {
			get { return this ["Time"]; }
			set { this ["Time"] = value; }
		}

		public string Host {
			get { return this ["Host"]; }
			set { this ["Host"] = value; }
		}
		
		public string Sender {
			get { return this ["Sender"]; }
			set { this ["Sender"] = value; }
		}
		
		public string Facility {
			get { return this ["Facility"]; }
			set { this ["Facility"] = value; }
		}
		
		public string PID {
			get { return this ["PID"]; }
			set { this ["PID"] = value; }
		}
		
		public string UID {
			get { return this ["UID"]; }
			set { this ["UID"] = value; }
		}
		
		public string GID {
			get { return this ["GID"]; }
			set { this ["GID"] = value; }
		}
		
		public string Level {
			get { return this ["Level"]; }
			set { this ["Level"] = value; }
		}
		
		public string Msg {
			get { return this ["Message"]; }
			set { this ["Message"] = value; }
		}

		[DllImport (Constants.SystemLibrary)]
		extern static int asl_set_query (IntPtr handle, string key, string value, int /* uint32_t */ op);
		
		public bool SetQuery (string key, Op op, string value)
		{
			return asl_set_query (Handle, key, value, (int) op) == 0;
		}
	}
}

#endif
