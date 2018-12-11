using System.Collections.Generic;
using System.Linq;
using Patient.Demographics.Common;

namespace Patient.Demographics.Service.FileUploads
{
    public class UploadValidatorFactory : IUploadValidatorFactory
    {
        private readonly IList<IUploadValidator> _validators;

        public UploadValidatorFactory(IList<IUploadValidator> validators)
        {
            _validators = validators;
        }

        public IUploadValidator GetValidator(BatchProcessTypes uploadType)
        {
            return _validators.First(v => v.Handles == uploadType);
        }
    }
}