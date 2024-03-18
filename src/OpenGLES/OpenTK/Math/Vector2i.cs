#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion

using System;
using System.Runtime.InteropServices;

#if !NET
namespace OpenTK {
	/// <summary>Represents a 2D vector using two int32 numbers.</summary>
	/// <remarks>
	/// The Vector2 structure is suitable for interoperation with unmanaged code requiring two consecutive ints.
	/// </remarks>
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct Vector2i : IEquatable<Vector2i> {
		#region Fields

		/// <summary>
		/// The X component of the Vector2i.
		/// </summary>
		public int X;

		/// <summary>
		/// The Y component of the Vector2i.
		/// </summary>
		public int Y;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Vector2.
		/// </summary>
		/// <param name="x">The x coordinate of the net Vector2.</param>
		/// <param name="y">The y coordinate of the net Vector2.</param>
		public Vector2i (int x, int y)
		{
			X = x;
			Y = y;
		}

		#endregion

		#region Public Members

		#region Instance

		/// <summary>
		/// Defines a unit-length Vector2i that points towards the X-axis.
		/// </summary>
		public static readonly Vector2i UnitX = new Vector2i (1, 0);

		/// <summary>
		/// Defines a unit-length Vector2i that points towards the Y-axis.
		/// </summary>
		public static readonly Vector2i UnitY = new Vector2i (0, 1);

		/// <summary>
		/// Defines a zero-length Vector2i.
		/// </summary>
		public static readonly Vector2i Zero = new Vector2i (0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector2i One = new Vector2i (1, 1);

		/// <summary>
		/// Defines the size of the Vector2i struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf<Vector2i> ();

		#endregion


		#region Add

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector2i Add (Vector2i a, Vector2i b)
		{
			Add (ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add (ref Vector2i a, ref Vector2i b, out Vector2i result)
		{
			result = new Vector2i (a.X + b.X, a.Y + b.Y);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector2i Subtract (Vector2i a, Vector2i b)
		{
			Subtract (ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract (ref Vector2i a, ref Vector2i b, out Vector2i result)
		{
			result = new Vector2i (a.X - b.X, a.Y - b.Y);
		}

		#endregion


		#region Clamp

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <returns>The clamped vector</returns>
		public static Vector2i Clamp (Vector2i vec, Vector2i min, Vector2i max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp (ref Vector2i vec, ref Vector2i min, ref Vector2i max, out Vector2i result)
		{
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
		}

		#endregion


		#region Operators

		/// <summary>
		/// Adds the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of addition.</returns>
		public static Vector2i operator + (Vector2i left, Vector2i right)
		{
			left.X += right.X;
			left.Y += right.Y;
			return left;
		}

		/// <summary>
		/// Subtracts the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of subtraction.</returns>
		public static Vector2i operator - (Vector2i left, Vector2i right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			return left;
		}

		/// <summary>
		/// Negates the specified instance.
		/// </summary>
		/// <param name="vec">Operand.</param>
		/// <returns>Result of negation.</returns>
		public static Vector2i operator - (Vector2i vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			return vec;
		}


		/// <summary>
		/// Compares the specified instances for equality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are equal; false otherwise.</returns>
		public static bool operator == (Vector2i left, Vector2i right)
		{
			return left.Equals (right);
		}

		/// <summary>
		/// Compares the specified instances for inequality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are not equal; false otherwise.</returns>
		public static bool operator != (Vector2i left, Vector2i right)
		{
			return !left.Equals (right);
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current Vector2i.
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ("({0}, {1})", X, Y);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode ();
		}

		#endregion

		#region public override bool Equals(object obj)

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals (object obj)
		{
			if (!(obj is Vector2i))
				return false;

			return this.Equals ((Vector2i) obj);
		}

		#endregion

		#endregion

		#endregion

		#region IEquatable<Vector2> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals (Vector2i other)
		{
			return
				X == other.X &&
				Y == other.Y;
		}

		#endregion
	}
}
#endif // !NET
