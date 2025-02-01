// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct NotificationData {

	/// <summary>
	/// Diff the constructor used in the bindings.
	/// </summary>
	internal enum ConstructorType {
		NotificationType,
		NotificationCenter,
		All
	}

	public string? Type { get; init; }
	public string? NotificationCenter { get; init; }

	public NotificationData (string? data, ConstructorType constructorType)
	{
		if (constructorType == ConstructorType.NotificationType) {
			Type = data;
		} else {
			NotificationCenter = data;
		}
	}

	public NotificationData (string? type, string? notificationCenter)
	{
		Type = type;
		NotificationCenter = notificationCenter;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out NotificationData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		ConstructorType constructorType = ConstructorType.NotificationType;
		string? notificationType = null;
		string? notificationCenter = null;

		switch (count) {
		case 0:
			break;
		case 1:
			// we have to diff constructors that take a single parameter, either a string or a type
			if (attributeData.ConstructorArguments [0].Value! is string notificationCenterValue) {
				constructorType = ConstructorType.NotificationCenter;
				notificationCenter = notificationCenterValue;
			} else {
				constructorType = ConstructorType.NotificationType;
				notificationType = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			}
			break;
		case 2:
			constructorType = ConstructorType.All;
			notificationType = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			notificationCenter = (string?) attributeData.ConstructorArguments [1].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = constructorType switch {
				ConstructorType.NotificationCenter => new (notificationCenter, ConstructorType.NotificationCenter),
				ConstructorType.NotificationType => new (notificationType, ConstructorType.NotificationType),
				_ => new (notificationType, notificationCenter)
			};
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "Type":
				notificationType = ((INamedTypeSymbol) value.Value!).ToDisplayString ();
				break;
			case "NotificationCenter":
				notificationCenter = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (notificationType, notificationCenter);
		return true;
	}

}
