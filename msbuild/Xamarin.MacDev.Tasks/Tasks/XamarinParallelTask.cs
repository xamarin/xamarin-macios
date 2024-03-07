using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class XamarinParallelTask : XamarinTask {
		// How many tasks we execute in parallel. Default is number of processors / 2.
		public string MaxDegreeOfParallelism { get; set; } = string.Empty;

		int GetMaxDegreeOfParallelism ()
		{
			if (!string.IsNullOrEmpty (MaxDegreeOfParallelism)) {
				if (int.TryParse (MaxDegreeOfParallelism, out var max))
					return max;
				Log.LogWarning (MSBStrings.W7121 /* Unable to parse the value '{0}' for the property 'MaxDegreeOfParallelism'. Falling back to the default value (number of processors / 2). */, MaxDegreeOfParallelism);
			}
			return Math.Max (Environment.ProcessorCount / 2, 1);
		}

		protected void ForEach<TSource> (IEnumerable<TSource> source, Action<TSource> body)
		{
			var options = new ParallelOptions { MaxDegreeOfParallelism = GetMaxDegreeOfParallelism () };
			Parallel.ForEach (source, options, body);
		}
	}
}
