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
	/// <summary>Represents a 4D vector using four int32 numbers.</summary>
	/// <remarks>
	/// The Vector4i structure is suitable for interoperation with unmanaged code requiring four consecutive int32_t.
	/// </remarks>
	[Serializable]
	[StructLayout (LayoutKind.Sequential)]
	public struct Vector4i : IEquatable<Vector4i> {
		#region Fields

		/// <summary>
		/// The X component of the Vector4i.
		/// </summary>
		public int X;

		/// <summary>
		/// The Y component of the Vector4i.
		/// </summary>
		public int Y;

		/// <summary>
		/// The Z component of the Vector4i.
		/// </summary>
		public int Z;

		/// <summary>
		/// The W component of the Vector4i.
		/// </summary>
		public int W;

		/// <summary>
		/// Defines a unit-length Vector4i that points towards the X-axis.
		/// </summary>
		public static Vector4i UnitX = new Vector4i (1, 0, 0, 0);

		/// <summary>
		/// Defines a unit-length Vector4i that points towards the Y-axis.
		/// </summary>
		public static Vector4i UnitY = new Vector4i (0, 1, 0, 0);

		/// <summary>
		/// Defines a unit-length Vector4i that points towards the Z-axis.
		/// </summary>
		public static Vector4i UnitZ = new Vector4i (0, 0, 1, 0);

		/// <summary>
		/// Defines a unit-length Vector4i that points towards the W-axis.
		/// </summary>
		public static Vector4i UnitW = new Vector4i (0, 0, 0, 1);

		/// <summary>
		/// Defines a zero-length Vector4i.
		/// </summary>
		public static Vector4i Zero = new Vector4i (0, 0, 0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector4i One = new Vector4i (1, 1, 1, 1);

		/// <summary>
		/// Defines the size of the Vector4i struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf<Vector4i> ();

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Vector4i.
		/// </summary>
		/// <param name="x">The x component of the Vector4i.</param>
		/// <param name="y">The y component of the Vector4i.</param>
		/// <param name="z">The z component of the Vector4i.</param>
		/// <param name="w">The z component of the Vector4i.</param>
		public Vector4i (int x, int y, int z, int w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>
		/// Constructs a new Vector4i from the given Vector2i.
		/// </summary>
		/// <param name="v">The Vector2i to copy components from.</param>
		public Vector4i (Vector2i v)
		{
			X = v.X;
			Y = v.Y;
			Z = 0;
			W = 0;
		}

		/// <summary>
		/// Constructs a new Vector4i from the given Vector3i.
		/// </summary>
		/// <param name="v">The Vector3i to copy components from.</param>
		public Vector4i (Vector3i v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = 0;
		}

		/// <summary>
		/// Constructs a new Vector4i from the specified Vector3i and W component.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		/// <param name="w">The W component of the new Vector4i.</param>
		public Vector4i (Vector3i v, int w)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = w;
		}

		/// <summary>
		/// Constructs a new Vector4i from the given Vector4i.
		/// </summary>
		/// <param name="v">The Vector4i to copy components from.</param>
		public Vector4i (Vector4i v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		#endregion

		#region Sub

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector4i Sub (Vector4i a, Vector4i b)
		{
			a.X -= b.X;
			a.Y -= b.Y;
			a.Z -= b.Z;
			a.W -= b.W;
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Sub (ref Vector4i a, ref Vector4i b, out Vector4i result)
		{
			result.X = a.X - b.X;
			result.Y = a.Y - b.Y;
			result.Z = a.Z - b.Z;
			result.W = a.W - b.W;
		}

		#endregion


		#region Add

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector4i Add (Vector4i a, Vector4i b)
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
		public static void Add (ref Vector4i a, ref Vector4i b, out Vector4i result)
		{
			result = new Vector4i (a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector4i Subtract (Vector4i a, Vector4i b)
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
		public static void Subtract (ref Vector4i a, ref Vector4i b, out Vector4i result)
		{
			result = new Vector4i (a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}

		#endregion


		#region Min

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector4i Min (Vector4i a, Vector4i b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			a.W = a.W < b.W ? a.W : b.W;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void Min (ref Vector4i a, ref Vector4i b, out Vector4i result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
			result.W = a.W < b.W ? a.W : b.W;
		}

		#endregion

		#region Max

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector4i Max (Vector4i a, Vector4i b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			a.W = a.W > b.W ? a.W : b.W;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void Max (ref Vector4i a, ref Vector4i b, out Vector4i result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
			result.W = a.W > b.W ? a.W : b.W;
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
		public static Vector4i Clamp (Vector4i vec, Vector4i min, Vector4i max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			vec.Z = vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			vec.W = vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp (ref Vector4i vec, ref Vector4i min, ref Vector4i max, out Vector4i result)
		{
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			result.Z = vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			result.W = vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
		}

		#endregion


		#region Swizzle

		/// <summary>
		/// Gets or sets an OpenTK.Vector2 with the X and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Xy { get { return new Vector2i (X, Y); } set { X = value.X; Y = value.Y; } }

		/// <summary>
		/// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Xyz { get { return new Vector3i (X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }

		#endregion

		#region Operators

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4i operator + (Vector4i left, Vector4i right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			left.W += right.W;
			return left;
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4i operator - (Vector4i left, Vector4i right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			left.W -= right.W;
			return left;
		}

		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4i operator - (Vector4i vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			vec.W = -vec.W;
			return vec;
		}


		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator == (Vector4i left, Vector4i right)
		{
			return left.Equals (right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator != (Vector4i left, Vector4i right)
		{
			return !left.Equals (right);
		}

		/// <summary>
		/// Returns a pointer to the first element of the specified instance.
		/// </summary>
		/// <param name="v">The instance.</param>
		/// <returns>A pointer to the first element of v.</returns>
		public static explicit operator IntPtr (Vector4i v)
		{
			unsafe {
				return (IntPtr) (&v.X);
			}
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current Vector4i.
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3})", X, Y, Z, W);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode () ^ W.GetHashCode ();
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
			if (!(obj is Vector4i))
				return false;

			return this.Equals ((Vector4i) obj);
		}

		#endregion

		#endregion

		#region IEquatable<Vector4i> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals (Vector4i other)
		{
			return
				X == other.X &&
				Y == other.Y &&
				Z == other.Z &&
				W == other.W;
		}

		#endregion
	}
}
#endif // !NET
