
using System;
using System.Collections.Generic;

namespace Patient.Demographics.Common.Settings
{
    public interface IDocumentStoreSettings
    {
        IEnumerable<Uri> DocumentStoreUris { get; }
        string Index { get; }
    }
}
