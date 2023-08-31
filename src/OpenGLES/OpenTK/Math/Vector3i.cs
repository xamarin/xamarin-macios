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
using System.Xml.Serialization;

#if !NET
namespace OpenTK {
	/// <summary>
	/// Represents a 3D vector using three int32 numbers.
	/// </summary>
	/// <remarks>
	/// The Vector3i structure is suitable for interoperation with unmanaged code requiring three consecutive ints.
	/// </remarks>
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct Vector3i : IEquatable<Vector3i> {
		#region Fields

		/// <summary>
		/// The X component of the Vector3i.
		/// </summary>
		public int X;

		/// <summary>
		/// The Y component of the Vector3i.
		/// </summary>
		public int Y;

		/// <summary>
		/// The Z component of the Vector3i.
		/// </summary>
		public int Z;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Vector3i.
		/// </summary>
		/// <param name="x">The x component of the Vector3i.</param>
		/// <param name="y">The y component of the Vector3i.</param>
		/// <param name="z">The z component of the Vector3i.</param>
		public Vector3i (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Constructs a new Vector3i from the given Vector2i.
		/// </summary>
		/// <param name="v">The Vector2i to copy components from.</param>
		public Vector3i (Vector2i v)
		{
			X = v.X;
			Y = v.Y;
			Z = 0;
		}

		/// <summary>
		/// Constructs a new Vector3i from the given Vector3i.
		/// </summary>
		/// <param name="v">The Vector3i to copy components from.</param>
		public Vector3i (Vector3i v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		#endregion

		#region Public Members

		#region Instance


		#region Static

		#region Fields

		/// <summary>
		/// Defines a unit-length Vector3i that points towards the X-axis.
		/// </summary>
		public static readonly Vector3i UnitX = new Vector3i (1, 0, 0);

		/// <summary>
		/// Defines a unit-length Vector3i that points towards the Y-axis.
		/// </summary>
		public static readonly Vector3i UnitY = new Vector3i (0, 1, 0);

		/// <summary>
		/// /// Defines a unit-length Vector3i that points towards the Z-axis.
		/// </summary>
		public static readonly Vector3i UnitZ = new Vector3i (0, 0, 1);

		/// <summary>
		/// Defines a zero-length Vector3i.
		/// </summary>
		public static readonly Vector3i Zero = new Vector3i (0, 0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector3i One = new Vector3i (1, 1, 1);

		/// <summary>
		/// Defines the size of the Vector3i struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf<Vector3i> ();

		#endregion


		#endregion

		#region Add

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector3i Add (Vector3i a, Vector3i b)
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
		public static void Add (ref Vector3i a, ref Vector3i b, out Vector3i result)
		{
			result = new Vector3i (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector3i Subtract (Vector3i a, Vector3i b)
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
		public static void Subtract (ref Vector3i a, ref Vector3i b, out Vector3i result)
		{
			result = new Vector3i (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		#endregion


		#region ComponentMin

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector3i ComponentMin (Vector3i a, Vector3i b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void ComponentMin (ref Vector3i a, ref Vector3i b, out Vector3i result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
		}

		#endregion

		#region ComponentMax

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector3i ComponentMax (Vector3i a, Vector3i b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void ComponentMax (ref Vector3i a, ref Vector3i b, out Vector3i result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
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
		public static Vector3i Clamp (Vector3i vec, Vector3i min, Vector3i max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			vec.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp (ref Vector3i vec, ref Vector3i min, ref Vector3i max, out Vector3i result)
		{
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			result.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
		}

		#endregion


		#endregion

		#region Swizzle

		/// <summary>
		/// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Xy { get { return new Vector2i (X, Y); } set { X = value.X; Y = value.Y; } }

		#endregion

		#region Operators

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator + (Vector3i left, Vector3i right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			return left;
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator - (Vector3i left, Vector3i right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			return left;
		}

		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator - (Vector3i vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			return vec;
		}


		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator == (Vector3i left, Vector3i right)
		{
			return left.Equals (right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator != (Vector3i left, Vector3i right)
		{
			return !left.Equals (right);
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current Vector3i.
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2})", X, Y, Z);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode ();
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
			if (!(obj is Vector3i))
				return false;

			return this.Equals ((Vector3i) obj);
		}

		#endregion

		#endregion

		#endregion

		#region IEquatable<Vector3i> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals (Vector3i other)
		{
			return
				X == other.X &&
				Y == other.Y &&
				Z == other.Z;
		}

		#endregion
	}
}
#endif // !NET
