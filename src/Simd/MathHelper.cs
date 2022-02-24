#nullable enable

#if NET
namespace CoreGraphics {
	static class MathHelper {
		/// <summary>
		/// Returns an approximation of the inverse square root of left number.
		/// </summary>
		/// <param name="x">A number.</param>
		/// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
		/// <remarks>
		/// This is an improved implementation of the the method known as Carmack's inverse square root
		/// which is found in the Quake III source code. This implementation comes from
		/// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
		/// http://www.beyond3d.com/content/articles/8/
		/// </remarks>
		internal static float InverseSqrtFast (float x)
		{
			unsafe
			{
				float xhalf = 0.5f * x;
				int i = *(int*) &x;              // Read bits as integer.
				i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
				x = *(float*) &i;                // Convert bits back to float
				x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
				return x;
			}
		}

		/// <summary>
		/// Returns an approximation of the inverse square root of left number.
		/// </summary>
		/// <param name="x">A number.</param>
		/// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
		/// <remarks>
		/// This is an improved implementation of the the method known as Carmack's inverse square root
		/// which is found in the Quake III source code. This implementation comes from
		/// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
		/// http://www.beyond3d.com/content/articles/8/
		/// </remarks>
		internal static double InverseSqrtFast (double x)
		{
			return InverseSqrtFast ((float) x);
		}
	}
}
#endif // !NET
