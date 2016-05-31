using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Clang;
using Clang.Ast;

using Sharpie;
using Sharpie.Tooling;
using Sharpie.Bind;
using Sharpie.Mono.CSharp;


namespace APITestTool
{
	class PropertyGetSetMismatchVisitor : TestVistorBase
	{
		bool Skip (PropertyInfo managedProperty)
		{
			switch (currentType.Name) {
				case "NSData":
					if (managedProperty.Name == "Length")
						return true;
					return false;
				case "NSObjectController":
				case "NSTreeController":
					if (managedProperty.Name == "SelectedObjects")
						return true;
					return false;
				case "AVComposition":
					if (managedProperty.Name == "NaturalSize")
						return true;
					return false;
				case "AVVideoComposition":
					if (managedProperty.Name == "CustomVideoCompositorClass")
						return true;
					return false;
				case "AVVideoCompositionInstruction":
					switch (managedProperty.Name)
					{
						case "TimeRange":
						case "LayerInstructions":
						case "EnablePostProcessing":
							return true;
						default:
							return false;
					}
				case "AVMetadataItem":
					switch (managedProperty.Name)
					{
						case "MetadataIdentifier":
						case "ExtendedLanguageTag":
						case "Locale":
						case "Time":
						case "Duration":
						case "DataType":
						case "Value":
						case "ExtraAttributes":
							return true;
						default:
							return false;
					}
				case "AVTimedMetadataGroup":
					switch (managedProperty.Name)
					{
						case "TimeRange":
						case "Items":
							return true;
						default:
							return false;
					}
				case "CBService":
					switch (managedProperty.Name)
					{
						case "Primary":
						case "IncludedServices":
						case "Characteristics":
							return true;
						default:
							return false;
					}
				case "CBCharacteristic":
					switch (managedProperty.Name)
					{
						case "UUID":
						case "Properties":
						case "Value":
						case "Descriptors":
							return true;
						default:
							return false;
					}
				case "GKAchievement":
					if (managedProperty.Name == "LastReportedDate")
						return true;
					return false;
				case "GKLeaderboard":
					if (managedProperty.Name == "GroupIdentifier")
						return true;
					return false;
				case "SKPayment":
					if (managedProperty.Name == "RequestData")
						return true;
					return false;
				case "NSParagraphStyle":
					switch (managedProperty.Name)
					{
						case "Alignment":
						case "BaseWritingDirection":
						case "DefaultTabInterval":
						case "FirstLineHeadIndent":
						case "HeadIndent":
						case "HeaderLevel":
						case "HyphenationFactor":
						case "LineBreakMode":
						case "LineHeightMultiple":
						case "LineSpacing":
						case "MaximumLineHeight":
						case "MinimumLineHeight":
						case "ParagraphSpacing":
						case "ParagraphSpacingBefore":
						case "TabStops":
						case "TailIndent":
						case "TextBlocks":
						case "TextLists":
						case "TighteningFactorForTruncation":
							return true;
						default:
							return false;
					}
			}
			return false;
		}

		void CheckPropertyStatus (bool hasNative, bool hasManaged, PropertyInfo managedProperty, string propertyErrorString)
		{
			if (hasNative != hasManaged) {
				// If they don't match, we have two cases.
				// Native has, we don't - Missing binding warning.
				// Native doesn't have but managed does? Either:
				//   Has NotImplemented in binding (which we can't tell easily without walking generated code for thrown exception)
				//   An actual binding bug. We use a Skip list of manually verified properites
				if (hasNative) { 
					throw new InvalidOperationException (string.Format("Possible missing binding - {0}", propertyErrorString));
				}
				else {
					if (!Skip (managedProperty))
						throw new InvalidOperationException (string.Format("Possible binding bug - {0}", propertyErrorString));
				}
			}
		}

		public override void VisitObjCPropertyDecl (ObjCPropertyDecl decl)
		{
			if (currentInterfaceDecl == null || currentType == null)
				return;

			PropertyInfo managedProperty = FindProperty (decl);
			if (managedProperty == null)
				return;

			bool nativeHasGetter = decl.Getter != null;
			bool nativeHasSetter = decl.Setter != null;
			bool managedHasGetter = managedProperty.CanRead;
			bool managedHasSetter = managedProperty.CanWrite;
			CheckPropertyStatus (nativeHasGetter, managedHasGetter, managedProperty, string.Format ("{0} getter on {1}", decl, currentType.Name));
			CheckPropertyStatus (nativeHasSetter, managedHasSetter, managedProperty, string.Format ("{0} setter on {1}", decl, currentType.Name));
		}
	}
}