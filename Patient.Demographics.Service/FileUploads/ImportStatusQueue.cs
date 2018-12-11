using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Commands;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Service.Exceptions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Patient.Demographics.Events.BatchProces;
using Patient.Demographics.Commands.Uploads;

namespace Patient.Demographics.Service.FileUploads
{
    public class ImportStatusQueue : IUploadStatusChangeQueue
    {
        private readonly IUploadImporterFactory _importerFactory;
        private readonly IBatchProcessRepository _uploadRepository;
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILogger _logger;

        public ImportStatusQueue
            (IUploadImporterFactory importerFactory,
            IBatchProcessRepository uploadRepository,
            IUploadsNotifier uploadsNotifier,
            ICommandExecutor commandExecutor,
            ILogger logger)
        {
            _importerFactory = importerFactory;
            _uploadRepository = uploadRepository;
            _uploadsNotifier = uploadsNotifier;
            _commandExecutor = commandExecutor;
            _logger = logger;
        }

        public BatchProcessStatuses UploadStatus => BatchProcessStatuses.ReadyToImport;

        public async Task Complete(BatchProcessStatusChangedEvent context)
        {
            if (context.StatusAfter == BatchProcessStatuses.ReadyToImport)
            {
                var uploadId = context.AggregateId;

                BatchProcessDto upload = null;

                try
                {
                    DebugConsole.WriteLine($"Importing upload {uploadId}");

                    upload = await _uploadRepository.GetBatchProcessAsync(uploadId);

                    if (upload == null)
                    {
                        throw new UploadImportException(context.AggregateId.ToString(), "Upload not found");
                    }
                    var timer = new Stopwatch();
                    timer.Start();
                    await ChangeUploadStatus(upload, BatchProcessStatuses.ImportInProgress, context.UpdatedBy);
                    DebugConsole.WriteLine($"Upload: {uploadId} - status changed to 'ImportInProgress'.");

                    await CallImporter(upload, uploadId, context.UpdatedBy);
                    DebugConsole.WriteLine($"Upload: {uploadId} - import complete.");

                    timer.Stop();

                    DebugConsole.WriteLine($"Time spent {timer.ElapsedMilliseconds}");

                    await ChangeUploadStatus(upload, BatchProcessStatuses.Imported, context.UpdatedBy);
                    DebugConsole.WriteLine($"Upload: {uploadId} - status changed to 'Imported'.");

                    if (upload.AutoFile)
                    {
                        DebugConsole.WriteLine($"Upload: {uploadId} - is auto file, starting auto file upload process.");

                        //await _autoFileUploadService.ProcessImportedFile(uploadId);
                    }
                    else
                    {
                        DebugConsole.WriteLine($"Upload: {uploadId} - sending import complete notification.");

                        await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.Imported, string.Empty, 100);

                        DebugConsole.WriteLine($"Upload: {uploadId} - import complete notification sent.");
                    }
                }
                catch (Exception exception)
                {
                    DebugConsole.WriteLine($"Upload: {uploadId} - error occurred: {exception}.");

                    await _uploadsNotifier.Notify(uploadId, BatchProcessStatuses.Error, "An error occurred while applying changes from the contents of the file", 1);
                    await _logger.LogExceptionAsync(exception);

                    if (upload != null)
                    {
                        await ChangeUploadStatus(upload, BatchProcessStatuses.Error, context.UpdatedBy);
                    }
                }
            }
        }

        private async Task CallImporter(BatchProcessDto upload, Guid uploadId, Guid userId)
        {
            var importer = _importerFactory.GetImporter(upload.BatchProcessType);
            await importer.Import(uploadId, userId);
        }

        private async Task ChangeUploadStatus(BatchProcessDto upload, BatchProcessStatuses uploadStatus, Guid userId)
        {
            var command = new ChangeUploadStatusCommand
            {
                Id = upload.Id,
                NewStatus = uploadStatus,
                CommandIssuedByUserId = userId
            };

            await _commandExecutor.ExecuteAsync(command);
        }
    }
}