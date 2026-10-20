// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

using TrimmerMetadataLibrary;

var model = new MetadataModel (1, "description");
Console.WriteLine (model.Description);
Console.WriteLine (model.WithValue (2));
