using Patient.Demographics.Common;
using Patient.Demographics.Commands;
using Patient.Demographics.Commands.Users;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Service.Common;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Data.Entities.BatchProcess;

namespace Patient.Demographics.Service.FileUploads.Importers.User
{
    public interface ICategoryImportPersistance
    {
        Task SaveAsync(BatchProcessDto upload, BatchProcessMasterDto productMaster, IList<RowToImport> rowsToImport, Guid commandIssuedByUserId);
    }

    public class CategoryImportPersistance : ImportPersistanceBase, ICategoryImportPersistance
    {
        private readonly IUserRepository _userRepository;
        private readonly ISqlDataContext _dataContext;
        private readonly IUploadsNotifier _uploadsNotifier;
        private readonly ICommandExecutor _commandExecutor;
        private ILogger _logger;

        private int _totalNumberOfRecords;
        private int _numberOfRecordsUpdated;
        private Guid _uploadId;
        private int _notifierRate;

        private IDictionary<string, Guid> _userIdentifiers;

        public CategoryImportPersistance(IUserRepository userRepository,
            IUploadsNotifier uploadsNotifier,
            ISqlDataContext dataContext,
            ICommandExecutor commandExecutor, ILogger logger)
        {
            _userRepository = userRepository;
            _uploadsNotifier = uploadsNotifier;
            _dataContext = dataContext;
            _commandExecutor = commandExecutor;
            _logger = logger;
        }

        public async Task SaveAsync(BatchProcessDto upload, BatchProcessMasterDto orgMaster, IList<RowToImport> rowsToImport, Guid commandIssuedByUserId)
        {
            var stopwatch = Stopwatch.StartNew();

            _numberOfRecordsUpdated = 0;
            _totalNumberOfRecords = rowsToImport.Count;
            _uploadId = upload.Id;
            _notifierRate = NotificationUtilities.CalculateNotifierRate(_totalNumberOfRecords);

            _userIdentifiers = await _userRepository.GetIdentifiersAsync();

            var usersToImportDictionary = rowsToImport.ToDictionary(p => p.ExternalIdentifier, p => p, StringComparer.OrdinalIgnoreCase);

            var usersToAdd = usersToImportDictionary.Where(x => !_userIdentifiers.ContainsKey(x.Key)).Select(x => x.Value).ToList();
            var usersToUpdate = usersToImportDictionary.Where(x => _userIdentifiers.ContainsKey(x.Key)).Select(x => x.Value).ToList();
            var usersToDelete = _userIdentifiers.Where(x => !usersToImportDictionary.ContainsKey(x.Key)).Select(x => x.Value).ToList();

            if (upload.AppendNewRecords)
            {
                //await UpdateRecordsUpdated();

                foreach (var row in usersToAdd)
                {
                    //var command = MapRowToCreateUserCommand(row, commandIssuedByUserId);
                    //await _commandExecutor.ExecuteAsync(command);

                    //await _dataContext.UpdateAsync<UploadRowEntity>(u => u.UploadId == upload.Id && u.RowNumber == row.RowNumber)
                    //                  .Set(u => new UploadRowEntity { Status = UploadRowStatuses.Imported.Name });

                    //await UpdateRecordsUpdated();
                }
            }

            if (upload.UpdateExistingRecords)
            {
                //await UpdateRecordsUpdated();

                foreach (var row in usersToUpdate)
                {
                    var existingUserId = _userIdentifiers[row.ExternalIdentifier];
                    //var userCommand = MapRowToUpdateUserCommand(existingUserId, row, commandIssuedByUserId);
                    //await _commandExecutor.ExecuteAsync(userCommand);

                    // Get Current User Organisations

                    await _dataContext.UpdateAsync<BatchProcessRecordEntity>(u => u.BatchProcessId == upload.Id && u.RowNumber == row.RowNumber).Set(u => new BatchProcessRecordEntity() { Status = BatchProcessRecordStatuses.Imported.Name });
                    //await UpdateRecordsUpdated();
                }
            }

            if (upload.DeleteMissingRecords && usersToDelete.Any())
            {
                // TODO: Use delete code for users when written
            }
            _numberOfRecordsUpdated = _totalNumberOfRecords;

            //await UpdateRecordsUpdated();
            _logger?.LogInfo($"user import took ${stopwatch.ElapsedMilliseconds} ms");
        }

        //private CreatePlatformUserViaUploadCommand MapRowToCreateUserCommand(RowToImport rowToImport, Guid commandIssuedByUserId)
        //{
        //    var username = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.UserName);
        //    var firstName = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.FirstName);
        //    var lastName = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.LastName);
        //    var emailAddress = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.EmailAddress);
        //    var mobileNumber = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.MobileNumber);
        //    var language = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.Language);
        //    var jobRoleId = GetJobRoleId(rowToImport);
        //    var organisationId = GetOrganisationId(rowToImport);

        //    var userIsOnline = GetUserOnlineStatus(rowToImport);

        //    var command = new CreatePlatformUserViaUploadCommand
        //    {
        //        CommandIssuedByUserId = commandIssuedByUserId,
        //        Username = username,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        Email = emailAddress,
        //        MobileNumber = mobileNumber,
        //        Culture = language,
        //        JobRoleId = jobRoleId,
        //        PrimaryOrganisationId = organisationId,
        //        AccountOnline = userIsOnline ?? true
        //    };

        //    return command;
        //}

        //private UpdateUserCommand MapRowToUpdateUserCommand(Guid userId, RowToImport rowToImport, Guid commandIssuedByUserId)
        //{
        //    var firstName = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.FirstName);
        //    var lastName = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.LastName);
        //    var emailAddress = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.EmailAddress);
        //    var mobileNumber = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.MobileNumber);
        //    var language = GetValueFromAttributes(rowToImport.Attributes, UsersBuiltInColumnNames.Language);

        //    var userIsOnline = GetUserOnlineStatus(rowToImport);

        //    return new UpdateUserCommand
        //    {
        //        UserId = userId,
        //        CommandIssuedByUserId = commandIssuedByUserId,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        Email = emailAddress,
        //        MobileNumber = mobileNumber,
        //        Culture = language,
        //        AccountOnline = userIsOnline
        //    };
        //}

        //private async Task UpdateRecordsUpdated()
        //{
        //    if ((_numberOfRecordsUpdated % _notifierRate == 0 || _numberOfRecordsUpdated == _totalNumberOfRecords) && _numberOfRecordsUpdated != 0)
        //    {
        //        DebugConsole.WriteLine($"Sending Import Status Update {_numberOfRecordsUpdated}/{_totalNumberOfRecords}");
        //        await _uploadsNotifier.Notify(_uploadId, UploadStatuses.ImportInProgress, _numberOfRecordsUpdated, _totalNumberOfRecords);
        //    }
        //    _numberOfRecordsUpdated++;
        //}
    }
}