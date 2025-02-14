#if NET
#if MONOMAC
using System;

using Foundation;
using ObjCRuntime;

namespace MediaExtension {
	public partial class MEByteSource {
		/// <summary>Read data asynchronously into a byte array.</summary>
		/// <param name="offset">The offset (relative to the start of the ifle) where to start reading.</param>
		/// <param name="data">The byte array to write the data into.</param>
		/// <param name="completionHandler">The delegate which is called when the read operation has completed.</param>
		/// <remarks>This overload will try to fill the <paramref name="data" /> byte array with data.</remarks>
		public void ReadData (long offset, byte [] data, MEByteSourceReadBytesCallback completionHandler)
		{
			ReadData ((nuint) data.Length, offset, data, completionHandler);
		}

		/// <summary>Read data asynchronously into a byte array.</summary>
		/// <param name="length">The number of bytes to read.</param>
		/// <param name="offset">The offset (relative to the start of the ifle) where to start reading.</param>
		/// <param name="data">The byte array to write the data into.</param>
		/// <param name="completionHandler">The delegate which is called when the read operation has completed.</param>
		public void ReadData (nuint length, long offset, byte [] data, MEByteSourceReadBytesCallback completionHandler)
		{
			if (data is null)
				ThrowHelper.ThrowArgumentNullException (nameof (data));

			if (length > (nuint) data.Length)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (length), length, $"length cannot be higher than the length of the data array.");

			unsafe {
				fixed (byte* dataPtr = data) {
					ReadData (length, offset, dataPtr, completionHandler);
				}
			}
		}

		/// <summary>Read data synchronously into a byte array.</summary>
		/// <param name="offset">The offset (relative to the start of the ifle) where to start reading.</param>
		/// <param name="data">The byte array to write the data into.</param>
		/// <param name="bytesRead">Upon return, will contain the number of read bytes.</param>
		/// <param name="error">Null if an error occurred, otherwise an <see cref="NSError" /> instance with information about the error. The error will be <see cref="MEError.EndOfStream" /> if the end was reached.</param>
		/// <returns>True if successful, false otherwise.</returns>
		/// <remarks>This overload will try to fill the <paramref name="data" /> byte array with data.</remarks>
		public bool ReadData (long offset, byte [] data, out nuint bytesRead, out NSError? error)
		{
			return ReadData ((nuint) data.Length, offset, data, out bytesRead, out error);
		}

		/// <summary>Read data synchronously into a byte array.</summary>
		/// <param name="length">The number of bytes to read.</param>
		/// <param name="offset">The offset (relative to the start of the ifle) where to start reading.</param>
		/// <param name="data">The byte array to write the data into.</param>
		/// <param name="bytesRead">Upon return, will contain the number of read bytes.</param>
		/// <param name="error">Null if an error occurred, otherwise an <see cref="NSError" /> instance with information about the error. The error will be <see cref="MEError.EndOfStream" /> if the end was reached.</param>
		/// <returns>True if successful, false otherwise.</returns>
		public bool ReadData (nuint length, long offset, byte [] data, out nuint bytesRead, out NSError? error)
		{
			if (data is null)
				ThrowHelper.ThrowArgumentNullException (nameof (data));

			if (length > (nuint) data.Length)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (length), length, $"length cannot be higher than the length of the data array.");

			unsafe {
				fixed (byte* dataPtr = data) {
					return ReadData (length, offset, dataPtr, out bytesRead, out error);
				}
			}
		}

	}
}
#endif // MONOMAC
#endif // NET
