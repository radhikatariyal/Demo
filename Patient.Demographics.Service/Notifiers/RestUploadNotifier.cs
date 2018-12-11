using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Service.FileUploads;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Patient.Demographics.Service.Notifiers
{
    public class RestUploadNotifier : IUploadsNotifier
    {
        private readonly IServiceSettings _configAppSettings;

        public RestUploadNotifier(IServiceSettings configAppSettings)
        {
            _configAppSettings = configAppSettings;
        }

        public async Task Notify(Guid uploadId, BatchProcessStatuses status, string message, int percentage)
        {
            var uploadStatusDto = new BatchProcessStatusDto()
            {
                UploadId = uploadId,
                Status = status.ToString(),
                ErrorMessage = message,
                Percentage = percentage
            };
            await PostStatus(uploadStatusDto);
        }

        public async Task Notify(Guid uploadId, BatchProcessStatuses status, int rowsProcessed, int totalRows, string message, int percentage)
        {
            var uploadStatusDto = new BatchProcessStatusDto()
            {
                UploadId = uploadId,
                Status = status.ToString(),
                NumberOfRowsProcessed = rowsProcessed,
                TotalNumberOfRows = totalRows,
                ErrorMessage = message,
                Percentage = percentage
            };
            await PostStatus(uploadStatusDto);
        }

        public async Task Notify(Guid uploadId, BatchProcessStatuses status, BatchProcessValidationResultSummaryDto validationResults, int percentage)
        {
            var uploadStatusDto = new BatchProcessStatusDto()
            {
                UploadId = uploadId,
                Status = status.ToString(),
                ValidationResults = validationResults,
                Percentage = percentage
            };
            await PostStatus(uploadStatusDto);
        }

        private HttpClient CreateUploadsNotificationHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_configAppSettings.ApiUrl) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private async Task PostStatus(BatchProcessStatusDto status)
        {
            using (var httpClient = CreateUploadsNotificationHttpClient())
            {
                var formatter = new JsonMediaTypeFormatter();
                formatter.SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                await httpClient.PostAsync(_configAppSettings.UploadStatusEndpointUrl, status, formatter);
            }
        }
    }
}