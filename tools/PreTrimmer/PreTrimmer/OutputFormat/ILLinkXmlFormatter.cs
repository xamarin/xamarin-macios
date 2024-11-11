
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Mono.Cecil;

namespace Mono.Linker {
	internal class LinkXmlFormatter {
		public static void WriteDescriptorsXml (
			string fileName,
			Dictionary<TypeDefinition, TypePreserve> preservedTypes,
			Dictionary<IMemberDefinition, List<IMemberDefinition>> preservedMembers,
			HashSet<IMetadataTokenProvider> marked)
		{
			var unhandledMembers = marked.Where (static m => m is not (TypeDefinition or FieldDefinition or EventDefinition or PropertyDefinition or MethodDefinition));
			if (unhandledMembers.Any ())
				throw new NotImplementedException ($"Unable to output non-member marked item to link xml: {unhandledMembers.First ().GetType ().FullName}");
			var markedMembers = marked
				.OfType<IMemberDefinition> ()
				.Where (m => m is not TypeDefinition)
				.GroupBy (m => m.DeclaringType)
				.ToDictionary (g => g.Key, g => g.ToList ());
			var allTypes = preservedTypes.Keys
				.Concat (preservedMembers.Keys.OfType<TypeDefinition> ())
				// .Concat (preservedMembers.Keys.OfType<MethodDefinition> ().Select (m => m.DeclaringType))
				.Concat (marked.OfType<TypeDefinition> ())
				.Concat (marked.OfType<FieldDefinition> ().Select (f => f.DeclaringType))
				.Concat (marked.OfType<EventDefinition> ().Select (f => f.DeclaringType))
				.Concat (marked.OfType<PropertyDefinition> ().Select (f => f.DeclaringType))
				.Distinct ();

			using var writer = XmlWriter.Create (fileName, new XmlWriterSettings () { Indent = true, OmitXmlDeclaration = true, IndentChars = "\t" });
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
								writer.WriteAttributeString ("preserve", typePreserve.ToString ().ToLowerInvariant());
							else
								writer.WriteAttributeString ("preserve", "nothing");

							if (marked.Contains (type))
								writer.WriteAttributeString ("required", "true");
							else
								writer.WriteAttributeString ("required", "false");

							if (markedMembers.TryGetValue (type, out var members)) {
								foreach (var member in members) {
									WriteMemberDefinition (writer, member, "true");
								}
							}
							if (preservedMembers.TryGetValue (type, out var membersPreservedOnThisType)) {
								foreach (var method in membersPreservedOnThisType) {
									if (marked.Contains (method))
										continue;
									WriteMemberDefinition (writer, method, "false");
								}
							}
						}
						writer.WriteEndElement ();
					}
				}
				writer.WriteEndElement ();
			}
			writer.WriteEndElement ();
			writer.Flush ();
		}


		static void WriteMemberDefinition (XmlWriter writer, IMemberDefinition member, string required)
		{
			string elementName = member switch {
				MethodDefinition => "method",
				FieldDefinition => "field",
				EventDefinition => "event",
				PropertyDefinition => "property",
				_ => throw new NotImplementedException ("Not sure what to do with this member type")
			};
			writer.WriteStartElement (elementName);
			{
				writer.WriteAttributeString ("signature", member.GetSignature ());
				writer.WriteAttributeString ("required", required);
			}
			writer.WriteEndElement ();
		}
	}

	file static class MemberDefinitionSignatureExtensions
	{
		// Corresponds to illink's ProcessLinkerXmlBase.Get[Method|Field|Event|Property]()
		public static string GetSignature(this IMemberDefinition member)
		{
			switch (member)
			{
				case MethodDefinition method:
					return GetMethodSignature(method, includeGenericParameters: true);
				case FieldDefinition field:
					return field.FieldType.FullName + ' ' + field.Name;
				case EventDefinition @event:
					return @event.EventType.FullName + ' ' + @event.Name;
				case PropertyDefinition property:
					return property.PropertyType.FullName + ' ' + property.Name;
				default:
					throw new NotImplementedException($"Not sure what to do with this member type: {member.GetType().FullName}");
			}
		}

		static string GetMethodSignature (MethodDefinition method, bool includeGenericParameters)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (method.ReturnType.FullName);
			sb.Append (' ');
			sb.Append (method.Name);
			if (includeGenericParameters && method.HasGenericParameters) {
				sb.Append ('`');
				sb.Append (method.GenericParameters.Count);
			}

			sb.Append ('(');
			if (method.HasMetadataParameters ()) {
				int i = 0;
				foreach (var p in method.Parameters) {
					if (i++ > 0)
						sb.Append (',');
					sb.Append (p.ParameterType.FullName);
				}
			}
			sb.Append (')');
			return sb.ToString ();
		}
	}
}
