using Patient.Demographics.Common;
using System;
using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads
{
    public class RowToImport
    {
        public Guid UploadId { get; set; }

        public int RowNumber { get; set; }

        public string ExternalIdentifier { get; set; }

        public IList<string> ParsedRow { get; set; }
    }
}