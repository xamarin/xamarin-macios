using System;
using System.IO;
using System.Text;

namespace Xharness.Logging {

	public abstract class Log : TextWriter, ILog {
		public ILogs Logs { get; private set; }
		public string Description { get; set; }
		public bool Timestamp { get; set; } = true;

		protected Log (ILogs logs)
		{
			Logs = logs;
		}

		protected Log (ILogs logs, string description)
		{
			Logs = logs;
			Description = description;
		}

		public abstract string FullPath { get; }

		public virtual void WriteImpl (string value)
		{
			Write (Encoding.UTF8.GetBytes (value));
		}

		public virtual void Write (byte [] buffer)
		{
			Write (buffer, 0, buffer.Length);
		}

		public virtual void Write (byte [] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public virtual StreamReader GetReader ()
		{
			throw new NotSupportedException ();
		}

		public override void Write (char value)
		{
			WriteImpl (value.ToString ());
		}

		public override void Write (string value)
		{
			if (Timestamp)
				value = DateTime.Now.ToString ("HH:mm:ss.fffffff") + " " + value;
			WriteImpl (value);
		}

		public override void WriteLine (string value)
		{
			Write (value + "\n");
		}

		public void WriteLine (StringBuilder value)
		{
			Write (value.ToString () + "\n");
		}
		public override void WriteLine (string format, params object [] args)
		{
			Write (string.Format (format, args) + "\n");
		}

		public override string ToString ()
		{
			return Description;
		}

		public override void Flush ()
		{
		}

		public override Encoding Encoding {
			get {
				return Encoding.UTF8;
			}
		}

		public static Log CreateAggregatedLog (params ILog [] logs)
		{
			return new AggregatedLog (logs);
		}

		// Log that will duplicate log output to multiple other logs.
		class AggregatedLog : Log {
			ILog [] logs;

			public AggregatedLog (params ILog [] logs)
				: base (null)
			{
				this.logs = logs;
			}

			public override string FullPath => throw new NotImplementedException ();

			public override void WriteImpl (string value)
			{
				foreach (var log in logs)
					log.WriteImpl (value);
			}

			public override void Write (byte [] buffer, int offset, int count)
			{
				foreach (var log in logs)
					log.Write (buffer, offset, count);
			}
		}
	}
}
