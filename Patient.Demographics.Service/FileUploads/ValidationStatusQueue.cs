using System;
using System.Threading.Tasks;
using Patient.Demographics.Commands;
using Patient.Demographics.Commands.Uploads;
using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Service.FileUploads;
using Patient.Demographics.Repositories.Repositories;

namespace Patient.Demographics.Service.FileUploads
{
    public class ValidationStatusQueue : IUploadStatusChangeQueue
    {
        private readonly IUploadValidatorFactory _validatorFactory;
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IBatchProcessRepository _uploadRepository;
        private readonly IProductMasterFileRepository _fileRepository;
         private readonly ILogger _logger;
        public ValidationStatusQueue(
            IUploadValidatorFactory validatorFactory,
            IUploadsNotifier uploadsNotifier,
            ICommandExecutor commandExecutor,
            IBatchProcessRepository uploadRepository,
            IProductMasterFileRepository fileRepository,
            ILogger logger
            )
        {
            _validatorFactory = validatorFactory;
            _uploadsNotifier = uploadsNotifier;
            _commandExecutor = commandExecutor;
            _uploadRepository = uploadRepository;
            _fileRepository = fileRepository;
            _logger = logger;
        }

        public BatchProcessStatuses UploadStatus => BatchProcessStatuses.ValidationInProgress;

        public async Task Complete(BatchProcessStatusChangedEvent @event)
        {
            var uploadId = @event.AggregateId;
            BatchProcessDto upload = null;

            _logger.LogInfo($"Received Validation Request For Upload {uploadId}");

            try
            {
                _logger.LogInfo($"Validating upload {uploadId}");

                await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ValidationInProgress, "Received the file", 1);

                upload = await _uploadRepository.GetBatchProcessAsync(uploadId);
                //var lastIndex = upload.FileName.LastIndexOf("_");

                //var filename = upload.FileName.Substring(0, lastIndex);
                //var file=await _fileRepository.GetByFileName(filename);
                //if(file.Count>0)
                //    upload.ProductFileId = file[0].Id;
                await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ValidationInProgress, $"Validation initiated for : {upload.FileName}", 2);

                await CallValidator(upload, @event.UpdatedBy);

                DebugConsole.WriteLine($"Upload: {uploadId} - validation complete.");
                _logger.LogInfo($"Upload: {uploadId} - validation complete.");

                await ChangeUploadStatus(uploadId, BatchProcessStatuses.Validated, @event.UpdatedBy);
                _logger.LogInfo($"Upload: {uploadId} - status changed to 'Validated'.");

                await ChangeUploadStatus(uploadId, BatchProcessStatuses.ReadyToImport, @event.UpdatedBy);
                await NotifyValidationCompleteAsync(uploadId);
                _logger.LogInfo($"Upload: {uploadId} - validation complete notification sent.");
            }
            catch (Exception exception)
            {
                DebugConsole.WriteLine($"Upload: {uploadId} - error occurred: {exception}.");
                //await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.Error, exception.Message, 100);
                await _logger.LogExceptionAsync(exception);

                if (upload != null)
                {
                    if (exception.Message == "Error validating upload - RowNumberOfColumnsNotCorrect")
                    {
                        await ChangeUploadStatus(uploadId, BatchProcessStatuses.ErrorInvalidFile, @event.UpdatedBy);
                        await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ErrorInvalidFile, exception.Message, 100);
                    }
                    if (exception.Message == "Error validating upload - UnrecognisedColumnName")
                    {
                        await ChangeUploadStatus(uploadId, BatchProcessStatuses.ErrorInvalidFile, @event.UpdatedBy);
                        await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ErrorInvalidFile, exception.Message, 100);

                    }
                    if (exception.Message == "Error validating upload - FileUploadedwithFormattingErrors")
                    {
                        await ChangeUploadStatus(uploadId, BatchProcessStatuses.FileUploadedwithFormattingErrors, @event.UpdatedBy);
                        await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.FileUploadedwithFormattingErrors, exception.Message, 100);

                    }
                    if (exception.Message == "Error validating upload - ErrorFileNotUploaded")
                    {
                        await ChangeUploadStatus(uploadId, BatchProcessStatuses.ErrorFileNotUploaded, @event.UpdatedBy);
                        await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.ErrorFileNotUploaded, exception.Message, 100);

                    }
                }
                else {
                    await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.Error, exception.Message, 100);

                }
            }
        }

        private async Task ChangeUploadStatus(Guid uploadId, BatchProcessStatuses uploadStatus, Guid updatedByUserId)
        {
            var command = new ChangeUploadStatusCommand
            {
                Id = uploadId,
                NewStatus = uploadStatus,
                CommandIssuedByUserId = updatedByUserId
            };

            await _commandExecutor.ExecuteAsync(command);
        }

        private async Task CallValidator(BatchProcessDto upload, Guid updatedByUserId)
        {
            var validator = _validatorFactory.GetValidator(upload.BatchProcessType);
            await validator.ValidateAsync(upload, updatedByUserId);
        }

        private async Task NotifyValidationCompleteAsync(Guid uploadId)
        {
            var uploadProgressDto = await _uploadRepository.GetBatchProcessProgressAsync(uploadId);
            await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.Validated, uploadProgressDto.ValidationSummary, 100);
        }
    }
}