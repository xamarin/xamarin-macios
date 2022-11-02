using System.Text;

using Foundation;

static class Extensions {
	// [NSData description] changed its format in Xcode 11 beta 1.
	// This method tries to hide that from tests, by producing the format from previous OS versions,
	// This way tests don't have to be modified (much).
	public static string ToStableString (this NSData data)
	{
		var sb = new StringBuilder ();
		sb.Append ('<');
		var bytes = data.ToArray ();
		for (var i = 0; i < bytes.Length; i++) {
			var b = bytes [i];
			sb.AppendFormat ("{0:x2}", b);
			if (((i + 1) % 4) == 0 && i + 1 < bytes.Length)
				sb.Append (' ');
		}
		sb.Append ('>');
		return sb.ToString ();
	}
}
