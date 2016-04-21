// Profile.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Xamarin.Pmcs
{
	public class Profile
	{
		public string Name { get; set; }
		public List<string> IgnorePaths { get; set; }

		public string CompilerExecutable { get; set; }
		public List<string> CompilerOptions { get; set; }

		public ReplacementCollection GlobalReplacements { get; set; }
		public ReplacementCollection EnumBackingTypeReplacements { get; set; }

		public Profile ()
		{
			IgnorePaths = new List<string> ();
			CompilerOptions = new List<string> ();
			GlobalReplacements = new ReplacementCollection (ReplacementContext.Global);
			EnumBackingTypeReplacements = new ReplacementCollection (ReplacementContext.EnumBackingType);
		}

		public override string ToString ()
		{
			var builder = new StringBuilder ();
			builder.AppendFormat ("    Name:                {0}\n", Name);
			builder.AppendFormat ("    Compiler executable: {0}\n", CompilerExecutable);
			builder.AppendFormat ("    Compiler options:    {0}\n", String.Join (" ", CompilerOptions));
			builder.AppendFormat ("    Ignore paths:        {0}\n", String.Join (" ", IgnorePaths));

			Action<string, ReplacementCollection> rToString = (header, repls) => {
				builder.Append (header);
				if (repls.Count == 0) {
					builder.AppendFormat ("(none)\n");
					return;
				}

				builder.Append ("\n");

				foreach (var r in repls) {
					builder.AppendFormat ("      {0}\n", r);
				}
			};

			rToString ("    Global replacements: ", GlobalReplacements);
			rToString ("    Enum replacements:   ", EnumBackingTypeReplacements);

			return builder.ToString ();
		}

		public void Load (Profile other)
		{
			if (other.Name != null)
				Name = other.Name;

			IgnorePaths.AddRange (other.IgnorePaths);
			CompilerExecutable = other.CompilerExecutable;
			CompilerOptions.AddRange (other.CompilerOptions);
			GlobalReplacements.AddRange (other.GlobalReplacements);
			EnumBackingTypeReplacements.AddRange (other.EnumBackingTypeReplacements);
		}

		public void Load (string path)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var builtinProfile = Profiles.BuiltinProfiles.Get (name);

			if (builtinProfile != null) {
				if (!File.Exists (path)) {
					Load (builtinProfile);
					return;
				}

				Console.Error.WriteLine ("WARNING (pmcs): profile filename '{0}' conflicts with " +
					"builtin-profile name; loading profile from file instead of " +
					"the builtin definition: {1}", name, path);
			}

			if (Name == null)
				Name = name;

			var reader = new XmlTextReader (path);

			while (reader.Read ()) {
				if (!reader.IsStartElement ())
					continue;

				switch (reader.Name) {
				case "pmcs":
					ReadPmcsElement (reader, Path.GetDirectoryName (path));
					break;
				default:
					throw new XmlException ("unexpected root element '" +
						reader.Name + "'; expected <pmcs>");
				}
			}
		}

		void ReadPmcsElement (XmlTextReader reader, string basePath)
		{
			while (reader.Read ()) {
				if (!reader.IsStartElement ())
					continue;

				switch (reader.Name) {
				case "replacements":
					ReadReplacementsElement (reader);
					break;
				case "compiler":
					ReadCompilerElement (reader);
					break;
				case "ignore":
					IgnorePaths.Add (reader.ReadString ());
					break;
				case "include":
					Load (Path.Combine (basePath, reader.ReadString ()));
					break;
				default:
					throw new XmlException ("<pmcs>: unexpected element <" +
						reader.Name + ">; expected <replacements> or <compiler>");
				}
			}
		}

		void ReadReplacementsElement (XmlTextReader reader)
		{
			ReplacementCollection replacements = null;
			string scope = null;

			if (reader.HasAttributes) {
				reader.MoveToNextAttribute ();
				if (reader.Name == "scope")
					scope = reader.Value;
			}

			switch (scope) {
			case "global":
				replacements = GlobalReplacements;
				break;
			case "enum":
				replacements = EnumBackingTypeReplacements;
				break;
			default:
				throw new XmlException ("<replacements> must have a 'scope' " +
					"attribute with a value of either 'global' or 'enum'");
			}

			while (reader.Read ()) {
				switch (reader.NodeType) {
				case XmlNodeType.EndElement:
					if (reader.Name == "replacements")
						return;
					break;
				case XmlNodeType.Element:
					switch (reader.Name) {
					case "regex":
					case "exact":
					case "prefix":
						var elementName = reader.Name;
						var replacement = new Replacement ();
						switch (reader.Name) {
						case "regex":
							replacement.Kind = ReplacementKind.Regex;
							break;
						case "exact":
							replacement.Kind = ReplacementKind.Exact;
							break;
						case "prefix":
							replacement.Kind = ReplacementKind.Prefix;
							break;
						}

						if (reader.HasAttributes) {
							while (reader.MoveToNextAttribute ()) {
								switch (reader.Name) {
								case "pattern":
									replacement.Pattern = reader.Value;
									break;
								case "replacement":
									replacement.Replace = reader.Value;
									break;
								default:
									throw new XmlException ("<" + elementName + ">: unexpected attribute '" +
										reader.Name + "'; expected 'pattern' or 'replacement'");
								}
							}
						}

						if (!replacement.IsValid)
							throw new XmlException ("<" + elementName +
								"> must have 'pattern' and 'replacement' attributes");

						replacements.Add (replacement);
						break;
					default:
						throw new XmlException ("<replacements>: unexpected element <" +
							reader.Name + ">; expected <regex>, <prefix>, or <exact>");
					}
					break;
				}
			}
		}

		void ReadCompilerElement (XmlTextReader reader)
		{
			while (reader.Read ()) {
				switch (reader.NodeType) {
				case XmlNodeType.EndElement:
					if (reader.Name == "compiler")
						return;
					break;
				case XmlNodeType.Element:
					switch (reader.Name) {
					case "executable":
						CompilerExecutable = reader.ReadString ();
						break;
					case "option":
						CompilerOptions.Add (reader.ReadString ());
						break;
					default:
						throw new XmlException ("<compiler>: unexpected element <" +
							reader.Name + ">; expected <executable> or <option>");
					}
					break;
				}
			}
		}
	}
}
