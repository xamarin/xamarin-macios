//
// DisposableObject.cs: A base class for many native data types without assuming any particular lifecycle.

// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2021 Microsoft Corp
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace ObjCRuntime {
	//
	// The DisposableObject class is intended to be a base class for many native data
	// data types, without assuming any particular lifecycle (might be reference counted,
	// might not be).
	//
	// It provides the common boilerplate for this kind of objects and the Dispose
	// pattern.
	//
	public abstract class DisposableObject : INativeObject, IDisposable {
		IntPtr handle;
		readonly bool owns;

		public IntPtr Handle {
			get => handle;
			protected set => InitializeHandle (value);
		}

		protected bool Owns { get; }

		protected DisposableObject ()
		{
		}

		protected DisposableObject (IntPtr handle, bool owns)
			: this (handle, owns, true)
		{
		}

		protected DisposableObject (IntPtr handle, bool owns, bool verify)
		{
			InitializeHandle (handle, verify);
			this.owns = owns;
		}

		~DisposableObject ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			ClearHandle ();
		}

		protected void ClearHandle ()
		{
			handle = IntPtr.Zero;
		}

		void InitializeHandle (IntPtr handle, bool verify)
		{
#if !COREBUILD
			if (verify && handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				throw new Exception ($"Could not initialize an instance of the type '{GetType ().FullName}': handle is null.\n" +
				    "It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.");
			}
#endif
			this.handle = handle;
		}

		protected virtual void InitializeHandle (IntPtr handle)
		{
			InitializeHandle (handle, true);
		}

		public IntPtr GetCheckedHandle ()
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowObjectDisposedException (this);
			return handle;
		}
	}
}
