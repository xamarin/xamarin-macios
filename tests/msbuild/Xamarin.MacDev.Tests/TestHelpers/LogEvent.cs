#nullable enable

using System.Text;

namespace Xamarin.Tests {
	public class LogEvent {
		public int ColumnNumber;
		public int EndColumnNumber;
		public int LineNumber;
		public int EndLineNumber;
		public string? Code;
		public string? SubCategory;
		public string? File;
		public string? ProjectFile;
		public string? Message;

		public override string ToString ()
		{
			// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
			var sb = new StringBuilder ();
			if (!string.IsNullOrEmpty (Code)) {
				sb.Append (Code);
				sb.Append (": ");
			}
			if (!string.IsNullOrEmpty (File)) {
				sb.Append (File);
				if (LineNumber != 0) {
					sb.Append ('(');
					sb.Append (LineNumber);
					sb.Append (')');
				}
				sb.Append (": ");
			}
			sb.Append (Message ?? "<no message>");
			return sb.ToString ();
		}
	}
}
