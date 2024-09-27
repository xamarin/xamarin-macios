
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Mono.Cecil;

namespace Mono.Linker {
	internal class XmlFormatter {
		public void OutputDescriptors (
			string fileName,
			Dictionary<TypeDefinition, TypePreserve> preservedTypes,
			Dictionary<IMemberDefinition, List<MethodDefinition>> preservedMethods,
			HashSet<IMetadataTokenProvider> marked)
		{
			if (marked.Where (static m => m is not TypeDefinition or FieldDefinition or EventDefinition or PropertyDefinition).Any ())
				throw new NotImplementedException ("Not sure what to do with non-member marked items");
			var markedMembers = marked
				.OfType<IMemberDefinition> ()
				.Where (m => m is not TypeDefinition)
				.GroupBy (m => m.DeclaringType)
				.ToDictionary (g => g.Key, g => g.ToList ());
			var allTypes = preservedTypes.Keys
				.Concat (preservedMethods.Keys.OfType<TypeDefinition> ())
				.Concat (preservedMethods.Keys.OfType<MethodDefinition> ().Select (m => m.DeclaringType))
				.Concat (marked.OfType<TypeDefinition> ())
				.Concat (marked.OfType<FieldDefinition> ().Select (f => f.DeclaringType))
				.Concat (marked.OfType<EventDefinition> ().Select (f => f.DeclaringType))
				.Concat (marked.OfType<PropertyDefinition> ().Select (f => f.DeclaringType))
				.Distinct ();

			var writer = XmlWriter.Create (fileName);
			writer.WriteStartElement ("linker");
			foreach (var grouping in allTypes.GroupBy (t => t.Module.Assembly)) {
				writer.WriteStartElement ("assembly");
				{
					writer.WriteAttributeString ("fullname", grouping.Key.FullName);
					foreach (var type in grouping) {
						writer.WriteStartElement ("type");
						{
							writer.WriteAttributeString ("fullname", type.FullName);

							if (preservedTypes.TryGetValue (type, out var typePreserve))
								writer.WriteAttributeString ("preserve", typePreserve.ToString ());
							else
								writer.WriteAttributeString ("preserve", "none");

							if (marked.Contains (type))
								writer.WriteAttributeString ("required", "true");

							if (markedMembers.TryGetValue (type, out var members)) {
								foreach (var member in members) {
									WriteMemberDefinition (writer, member);
								}
							}
							List<MethodDefinition>? methodsPreservedOnThisType = null;
							if (preservedMethods.TryGetValue (type, out methodsPreservedOnThisType)) {
								foreach (var method in methodsPreservedOnThisType) {
									if (marked.Contains (method))
										continue;
									writer.WriteStartElement ("method");
									writer.WriteAttributeString ("signature", method.FullName);
									writer.WriteAttributeString ("required", "false");
									writer.WriteEndElement ();
								}
							}
						}
						writer.WriteEndElement ();
					}
				}
				writer.WriteEndElement ();
			}
			writer.WriteEndElement ();
		}


		void WriteMemberDefinition (XmlWriter writer, IMemberDefinition method)
		{
			string elementName = method switch {
				MethodDefinition => "method",
				FieldDefinition => "field",
				EventDefinition => "event",
				PropertyDefinition => "property",
				_ => throw new NotImplementedException ("Not sure what to do with this member type")
			};
			writer.WriteStartElement (elementName);
			{
				writer.WriteAttributeString ("signature", method.FullName);
				writer.WriteAttributeString ("required", "true");
			}
			writer.WriteEndElement ();
		}
	}
}
