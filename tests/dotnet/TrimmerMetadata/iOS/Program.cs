// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.Json;

using TrimmerMetadataLibrary;

var model = new MetadataModel (1, "description");
Console.WriteLine (JsonSerializer.Serialize (model));
Console.WriteLine (model.WithValue (2));
