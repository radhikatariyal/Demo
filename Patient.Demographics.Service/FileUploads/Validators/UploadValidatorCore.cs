using System;
using System.Threading.Tasks;
using Patient.Demographics.Commands;
using Patient.Demographics.Commands.Uploads;

namespace Patient.Demographics.Service.FileUploads.Validators
{
    public abstract class UploadValidatorCore
    {
        private readonly ICommandExecutor _commandExecutor;

        public string n = "NotValidated";
        public string p = "Ok";
        
        protected UploadValidatorCore(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        protected async Task UpdateUploadWithValidationResultsAsync(Guid uploadId, int numberOfErrors, int numberToAdd, int numberUnchanged, int numberToUpdate,int numberOfDataRows,  Guid updatedByUserId)
        {
            var command = new UpdateUploadWithValidationResultsCommand
            {
                Id = uploadId,
                CommandIssuedByUserId = updatedByUserId,
                NumberOfErrors = numberOfErrors,
                NumberToAdd = numberToAdd,
                NumberUnchanged = numberUnchanged,
                NumberToUpdate = numberToUpdate,
                NumberOfDataRows = numberOfDataRows
            };

            await _commandExecutor.ExecuteAsync(command);
        }

        protected async Task UpdateUploadWithErrorsAsync(Guid uploadId, string errors, string errorCode,Guid updatedByUserId)
        {
            var command = new UpdateUploadWithErrorsCommand
            {
                Id = uploadId,
                CommandIssuedByUserId = updatedByUserId,
                Errors = errors,
                ErrorsCode = errorCode
            };

            await _commandExecutor.ExecuteAsync(command);
        }
    }
}
