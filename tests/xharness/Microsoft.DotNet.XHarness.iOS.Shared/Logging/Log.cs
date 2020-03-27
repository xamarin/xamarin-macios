using System;
using System.IO;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {

	public abstract partial class Log : ILog {

		public virtual Encoding Encoding => Encoding.UTF8;
		public string Description { get; set; }
		public bool Timestamp { get; set; } = true;

		protected Log (string description = null)
		{
			Description = description;
		}

		public virtual void Write (byte [] buffer, int offset, int count)
		{
			Write (Encoding.GetString (buffer, offset, count));
		}

		public void Write (string value)
		{
			if (Timestamp)
				value = DateTime.Now.ToString ("HH:mm:ss.fffffff") + " " + value;

			WriteImpl (value);
		}

		public void WriteLine (string value)
		{
			Write (value + "\n");
		}

		public void WriteLine (StringBuilder value)
		{
			Write (value.ToString () + "\n");
		}

		public void WriteLine (string format, params object [] args)
		{
			Write (string.Format (format, args) + "\n");
		}		

		public abstract string FullPath { get; }

		protected abstract void WriteImpl (string value);

		public abstract StreamReader GetReader ();

		public override string ToString () => Description;

		public abstract void Flush ();

		public abstract void Dispose ();
	}
}
