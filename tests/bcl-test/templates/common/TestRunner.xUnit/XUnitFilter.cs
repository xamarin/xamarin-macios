using System;
using System.Text;

namespace Xamarin.iOS.UnitTests.XUnit
{
	public class XUnitFilter
	{
		public string AssemblyName { get; private set; }
		public string SelectorName { get; private set; }
		public string SelectorValue { get; private set; }

		public bool Exclude { get; private set; }
		public XUnitFilterType FilterType { get; private set; }

		public static XUnitFilter CreateSingleFilter (string singleTestName, bool exclude, string assemblyName = null)
		{
			if (String.IsNullOrEmpty (singleTestName))
				throw new ArgumentException("must not be null or empty", nameof (singleTestName));

			return new XUnitFilter {
				AssemblyName = assemblyName,
				SelectorValue = singleTestName,
				FilterType = XUnitFilterType.Single,
				Exclude = exclude
			};
		}

		public static XUnitFilter CreateAssemblyFilter (string assemblyName, bool exclude)
		{
			if (String.IsNullOrEmpty (assemblyName))
				throw new ArgumentException("must not be null or empty", nameof (assemblyName));

			return new XUnitFilter {
				AssemblyName = assemblyName,
				FilterType = XUnitFilterType.Assembly,
				Exclude = exclude
			};
		}

		public static XUnitFilter CreateNamespaceFilter (string namespaceName, bool exclude, string assemblyName = null)
		{
			if (String.IsNullOrEmpty (namespaceName))
				throw new ArgumentException("must not be null or empty", nameof (namespaceName));

			return new XUnitFilter {
				AssemblyName = assemblyName,
				SelectorValue = namespaceName,
				FilterType = XUnitFilterType.Namespace,
				Exclude = exclude
			};
		}

		public static XUnitFilter CreateClassFilter (string className, bool exclude, string assemblyName = null)
		{
			if (String.IsNullOrEmpty (className))
				throw new ArgumentException("must not be null or empty", nameof (className));

			return new XUnitFilter {
				AssemblyName = assemblyName,
				SelectorValue = className,
				FilterType = XUnitFilterType.TypeName,
				Exclude = exclude
			};
		}

		public static XUnitFilter CreateTraitFilter (string traitName, string traitValue, bool exclude)
		{
			if (String.IsNullOrEmpty (traitName))
				throw new ArgumentException("must not be null or empty", nameof (traitName));

			return new XUnitFilter {
				AssemblyName = null,
				SelectorName = traitName,
				SelectorValue = traitValue ?? String.Empty,
				FilterType = XUnitFilterType.Trait,
				Exclude = exclude
			};
		}

		public override string ToString ()
		{
			var sb = new StringBuilder ("XUnitFilter [");

			sb.Append ($"Type: {FilterType}; ");
			sb.Append (Exclude ? "exclude" : "include");

			if (!String.IsNullOrEmpty (AssemblyName))
				sb.Append ($"; AssemblyName: {AssemblyName}");

			switch (FilterType) {
				case XUnitFilterType.Assembly:
					break;

				case XUnitFilterType.Namespace:
					AppendDesc ("Namespace", SelectorValue);
					break;

				case XUnitFilterType.Single:
					AppendDesc ("Method", SelectorValue);
					break;

				case XUnitFilterType.Trait:
					AppendDesc ("Trait name", SelectorName);
					AppendDesc ("Trait value", SelectorValue);
					break;

				case XUnitFilterType.TypeName:
					AppendDesc ("Class", SelectorValue);
					break;

				default:
					sb.Append ("; Unknown filter type");
					break;
			}
			sb.Append (']');

			return sb.ToString ();

			void AppendDesc (string name, string value)
			{
				if (String.IsNullOrEmpty (value))
					return;

				sb.Append ($"; {name}: {value}");
			}
		}
	}
}
