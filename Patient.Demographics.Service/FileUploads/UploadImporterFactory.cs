using System.Collections.Generic;
using System.Linq;
using Patient.Demographics.Common;

namespace Patient.Demographics.Service.FileUploads
{
    public class UploadImporterFactory : IUploadImporterFactory
    {
        private readonly IList<IUploadImporter> _importers;

        public UploadImporterFactory(IList<IUploadImporter> importers)
        {
            _importers = importers;
        }

        public IUploadImporter GetImporter(BatchProcessTypes uploadType)
        {
            return _importers.First(i => i.Handles == uploadType);
        }
    }
}