// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace TrimmerMetadataLibrary {
	public class MetadataModel {
		public int Identifier { get; }
		public string Description { get; }

		public MetadataModel (int identifier, string description)
		{
			Identifier = identifier;
			Description = description;
		}

		public int WithValue (int value)
		{
			return Identifier + value;
		}
	}
}
