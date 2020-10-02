using System;
using System.IO;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {

	public abstract partial class Log {

		public static ILog CreateAggregatedLog (params ILog [] logs)
		{
			return new AggregatedLog (logs);
		}

		// Log that will duplicate log output to multiple other logs.
		class AggregatedLog : Log {
			readonly ILog [] logs;

			public AggregatedLog (params ILog [] logs)
				: base (null)
			{
				this.logs = logs;
			}

			public override string FullPath => throw new NotImplementedException ();

			protected override void WriteImpl (string value)
			{
				foreach (var log in logs)
					log.Write (value);
			}

			public override void Write (byte [] buffer, int offset, int count)
			{
				foreach (var log in logs)
					log.Write (buffer, offset, count);
			}

			public override StreamReader GetReader ()
			{
				if (logs.Length > 0)
					return logs [0].GetReader ();
				return null;
			}

			public override void Flush ()
			{
				foreach (var log in logs)
					log.Flush ();
			}

			public override void Dispose ()
			{
				foreach (var log in logs)
					log.Dispose ();
			}
		}
	}
}
