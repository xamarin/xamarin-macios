//
// NSData.cs:
// Author:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2011 - 2014 Xamarin Inc
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
//
//

#nullable enable

using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Foundation {
	public partial class NSData : IEnumerable, IEnumerable<byte> {
		/// <summary>Copies the data into a new managed array.</summary>
		/// <returns>An independent copy of the data.</returns>
		public byte [] ToArray ()
		{
			var res = new byte [Length];
			if (Length > 0)
				Marshal.Copy (Bytes, res, 0, res.Length);
			return res;
		}

		/// <summary>Enumerator for the NSData contents.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++) {
				yield return Marshal.ReadByte (source, (int) i);
			}
		}

		IEnumerator<byte> IEnumerable<byte>.GetEnumerator ()
		{
			IntPtr source = Bytes;
			nuint top = Length;

			for (nuint i = 0; i < top; i++)
				yield return Marshal.ReadByte (source, (int) i);
		}

		/// <param name="s">String to wrap.</param>
		///         <summary>Creates an NSData for a C# string, the string is encoded in UTF8.</summary>
		///         <returns>Newly creates NSData.</returns>
		///         <remarks>
		///         </remarks>
		public static NSData FromString (string s)
		{
			return FromString (s, NSStringEncoding.UTF8);
		}

		/// <summary>Creates data by copying the contents of a managed array.</summary>
		/// <param name="buffer">The bytes to copy.</param>
		/// <returns>A new data object containing an independent copy of <paramref name="buffer" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer" /> is <see langword="null" />.</exception>
		public static NSData FromArray (byte [] buffer)
		{
			if (buffer is null)
				throw new ArgumentNullException (nameof (buffer));

			return FromArray (new ReadOnlySpan<byte> (buffer));
		}

		/// <summary>Creates data by copying the contents of a span.</summary>
		/// <param name="buffer">The bytes to copy.</param>
		/// <returns>A new data object containing an independent copy of <paramref name="buffer" />.</returns>
		/// <remarks>The bytes are copied during this call; the span is not retained.</remarks>
		public static NSData FromArray (ReadOnlySpan<byte> buffer)
		{
			if (buffer.Length == 0)
				return FromBytes (IntPtr.Zero, 0);

			unsafe {
				fixed (byte* ptr = buffer) {
					return FromBytes ((IntPtr) ptr, (nuint) buffer.Length);
				}
			}
		}

		/// <summary>Copies all the data into a destination span.</summary>
		/// <param name="buffer">The destination, which must be large enough to contain all the data.</param>
		/// <exception cref="ArgumentException"><paramref name="buffer" /> is smaller than <see cref="Length" />.</exception>
		/// <remarks>The copy completes during this call. Any bytes beyond the copied data are unchanged. Do not modify mutable data concurrently with this call.</remarks>
		public void GetBytes (Span<byte> buffer)
		{
			var length = Length;
			if ((nuint) buffer.Length < length)
				throw new ArgumentException ("The destination is too small to contain the data.", nameof (buffer));

			if (length == 0)
				return;

			unsafe {
				fixed (byte* ptr = buffer) {
					GetBytes ((IntPtr) ptr, length);
				}
			}
		}

		/// <summary>Copies a range of the data into a destination span.</summary>
		/// <param name="buffer">The destination, which must be large enough to contain the requested range.</param>
		/// <param name="range">The range of bytes to copy.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="range" /> is outside the data.</exception>
		/// <exception cref="ArgumentException"><paramref name="buffer" /> is smaller than the requested range.</exception>
		/// <remarks>The copy completes during this call. Any bytes beyond the copied data are unchanged. Do not modify mutable data concurrently with this call.</remarks>
		public void GetBytes (Span<byte> buffer, NSRange range)
		{
			var length = Length;
			if (range.Location < 0 || range.Length < 0 || (nuint) range.Location > length || (nuint) range.Length > length - (nuint) range.Location)
				throw new ArgumentOutOfRangeException (nameof (range));
			if ((nuint) buffer.Length < (nuint) range.Length)
				throw new ArgumentException ("The destination is too small to contain the requested range.", nameof (buffer));

			if (range.Length == 0)
				return;

			unsafe {
				fixed (byte* ptr = buffer) {
					GetBytes ((IntPtr) ptr, range);
				}
			}
		}

		/// <param name="stream">System.IO.Stream to wrap as an NSData.</param>
		///         <summary>Creates an NSData by loading the contents of the provided stream.</summary>
		///         <returns>Newly created NSData, or null if it the stream does not support reading or the stream throws an exception.</returns>
		///         <remarks>This method will load the contents of the stream starting at the current location in the stream.</remarks>
		public static NSData? FromStream (Stream stream)
		{
			if (stream is null)
				throw new ArgumentNullException (nameof (stream));

			if (!stream.CanRead)
				return null;

			long len = 8192;
			// do not try to access Length if CanSeek is false - it *should* throw NotSupportedException
			// but some stream implementation can return something, e.g. WebConnectionStream returns -1 [#15672]
			if (stream.CanSeek) {
				try {
					// https://trello.com/c/yyT3u2KY/84-nsdata-fromstream-optimization
					len = stream.Length - stream.Position;
				} catch {
				}
			}

			NSMutableData ret = NSMutableData.FromCapacity ((int) len);
			byte [] buffer = ArrayPool<byte>.Shared.Rent (32 * 1024);
			int n;
			try {
				unsafe {
					fixed (byte* ptr = buffer) {
						while ((n = stream.Read (buffer, 0, buffer.Length)) != 0)
							ret.AppendBytes ((IntPtr) ptr, (nuint) n);
					}
				}
			} catch {
				return null;
			} finally {
				ArrayPool<byte>.Shared.Return (buffer);
			}
			return ret;
		}

		//
		// Keeps a ref to the source NSData
		//
		unsafe class UnmanagedMemoryStreamWithRef : UnmanagedMemoryStream {
			NSData? source;

			protected NSData Source {
				get {
					if (source is null)
						throw new ObjectDisposedException (GetType ().FullName);
					return source;
				}
			}

			public UnmanagedMemoryStreamWithRef (NSData source) : base ((byte*) source.Bytes, (long) source.Length)
			{
				this.source = source;
			}

			/// <inheritdoc />
			protected override void Dispose (bool disposing)
			{
				source = null;
				base.Dispose (disposing);
			}
		}

		//
		// This variation of the class can be used with NSMutableData, but
		// displays an error if the NSMutableData changes while the stream is used
		//
		unsafe class UnmanagedMemoryStreamWithMutableRef : UnmanagedMemoryStreamWithRef {
			IntPtr base_address;

			public UnmanagedMemoryStreamWithMutableRef (NSData source) : base (source)
			{
				base_address = source.Bytes;
			}

			static void InvalidOperation ()
			{
				throw new InvalidOperationException ("The underlying NSMutableData changed while we were consuming data");
			}

			public override int Read ([InAttribute][OutAttribute] byte [] buffer, int offset, int count)
			{
				if (base_address != Source.Bytes)
					InvalidOperation ();

				return base.Read (buffer, offset, count);
			}

			public override int ReadByte ()
			{
				if (base_address != Source.Bytes)
					InvalidOperation ();

				return base.ReadByte ();
			}

			public override void Write (byte [] buffer, int offset, int count)
			{
				if (base_address != Source.Bytes)
					InvalidOperation ();
				base.Write (buffer, offset, count);
			}

			public override void WriteByte (byte value)
			{
				if (base_address != Source.Bytes)
					InvalidOperation ();
				base.WriteByte (value);
			}
		}

		/// <summary>Wraps the NSData into System.IO.Stream</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///           <para>Call Dispose on the returned Stream to release the reference to this NSData.</para>
		///           <example>
		///             <code lang="csharp lang-csharp"><![CDATA[
		/// void DumpImage (Stream output, UIImage img)
		/// {
		///     NSData data = img.AsPNG ();
		///     data.AsStream ().CopyTo (output);
		/// }
		/// ]]></code>
		///           </example>
		///         </remarks>
		public virtual Stream AsStream ()
		{
			unsafe {
				if (this is NSMutableData)
					return new UnmanagedMemoryStreamWithMutableRef (this);
				else
					return new UnmanagedMemoryStreamWithRef (this);
			}
		}

		/// <param name="s">String to wrap.</param>
		///         <param name="encoding">Encoding to use.</param>
		///         <summary>Creates an NSData for a C# string, the string is encoded using the specified encoding.</summary>
		///         <returns>Newly creates NSData.</returns>
		///         <remarks>
		///         </remarks>
		public static NSData FromString (string s, NSStringEncoding encoding)
		{
			using (var ns = new NSString (s))
				return ns.Encode (encoding);
		}

		/// <param name="s">String to convert.</param>
		/// <summary>Implicit conversion from string to an NSData encoded as UTF8.</summary>
		/// <returns>
		///         </returns>
		/// <remarks>
		///         </remarks>
		public static implicit operator NSData (string s)
		{
			return FromString (s, NSStringEncoding.UTF8);
		}

		/// <param name="encoding">The encoding to use to convert the contents.</param>
		///         <summary>Returns a string representation of the value of the current instance.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public NSString ToString (NSStringEncoding encoding)
		{
			return new NSString (this, encoding);
		}

		/// <summary>Returns a string representation of the value of the current instance.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public override string ToString ()
		{
			// not every NSData can be converted into a (valid) UTF8 string and:
			// * Your ToString override should not throw an exception
			//   -> http://msdn.microsoft.com/en-us/library/system.object.tostring(v=vs.110).aspx
			// * We want to show something valuable on XS watches while debugging
			try {
				using (var s = new NSString (this, NSStringEncoding.UTF8))
					return s.ToString ();
			} catch {
				return Description; // ObjC knows how to render NSData into a string
			}
		}

		/// <param name="file">To be added.</param>
		///         <param name="auxiliaryFile">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Save (string file, bool auxiliaryFile, out NSError? error)
		{
			return Save (file, auxiliaryFile ? NSDataWritingOptions.Atomic : (NSDataWritingOptions) 0, out error);
		}

		/// <param name="file">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Save (string file, NSDataWritingOptions options, out NSError? error)
		{
			unsafe {
				IntPtr val;
				IntPtr val_addr = (IntPtr) ((IntPtr*) &val);

				bool ret = _Save (file, (nint) (long) options, val_addr);
				error = Runtime.GetNSObject<NSError> (val);

				return ret;
			}
		}

		/// <param name="url">To be added.</param>
		///         <param name="auxiliaryFile">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Save (NSUrl url, bool auxiliaryFile, out NSError? error)
		{
			return Save (url, auxiliaryFile ? NSDataWritingOptions.Atomic : (NSDataWritingOptions) 0, out error);
		}

		/// <param name="url">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Save (NSUrl url, NSDataWritingOptions options, out NSError? error)
		{
			unsafe {
				IntPtr val;
				IntPtr val_addr = (IntPtr) ((IntPtr*) &val);

				bool ret = _Save (url, (nint) (long) options, val_addr);
				error = Runtime.GetNSObject<NSError> (val);

				return ret;
			}
		}

		/// <param name="idx">Index into the NSData.</param>
		/// <summary>Retrieves the byte at the specified position in the NSData object.</summary>
		/// <value>The value at that position, or an exception if you try to access data beyond its boundaries.</value>
		/// <remarks>
		///         </remarks>
		public virtual byte this [nint idx] {
			get {
				if (idx < 0 || (ulong) idx > Length)
					throw new ArgumentException (nameof (idx));
				return Marshal.ReadByte (new IntPtr (((long) Bytes) + idx));
			}

			set {
				throw new NotImplementedException ("NSData arrays can not be modified, use an NSMutableData instead");
			}
		}

		/// <summary>Create an <see cref="NSData" /> instance from the specified value type.</summary>
		/// <typeparam name="T">The type of the value type stored in returned <see cref="NSData" /> instance.</typeparam>
		/// <param name="value">The value to create the <see cref="NSData" /> instance from.</param>
		/// <returns>A new <see cref="NSData" /> instance from the specified value type.</returns>
		public unsafe static NSData? CreateFromValueType<T> (T? value) where T : unmanaged
		{
			if (!value.HasValue)
				return null;

			var size = (nuint) sizeof (T);
			if (size == 0)
				return new NSData ();

			var v = value.Value;
			unsafe {
				return NSData.FromBytes ((IntPtr) (&v), size);
			}
		}

		/// <summary>Create a value type from this <see cref="NSData" /> instance.</summary>
		/// <typeparam name="T">The type of the returned value type.</typeparam>
		/// <returns>A value type created from this <see cref="NSData" /> instance.</returns>
		/// <remarks>An exception will be throw if this <see cref="NSData" /> instance is smaller than the size of the value type <typeparamref name="T" />.</remarks>
		public unsafe T ToValueType<T> () where T : unmanaged
		{
			var size = (nuint) sizeof (T);
			if (size == 0)
				return default;

			if (Length < size)
				throw new ArgumentOutOfRangeException ($"The size of this NSData instance ({Length} bytes) is smaller than the size of the return type '{typeof (T).FullName}' ({size} bytes)");

			var rv = default (T);
			unsafe {
				NativeMemory.Copy ((void*) Bytes, &rv, size);
			}

			GC.KeepAlive (this);

			return rv;
		}
	}
}
