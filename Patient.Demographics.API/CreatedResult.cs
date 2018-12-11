using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Patient.Demographics.API
{
    public class CreatedResult : IHttpActionResult
    {
        private readonly Uri _location;
        private readonly HttpRequestMessage _request;

        public CreatedResult(Uri location, HttpRequestMessage request)
        {
            _location = location;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Created)
            {
                RequestMessage = _request
            };

            if (_location != null)
            {
                response.Headers.Location = _location;
            }
            
            return Task.FromResult(response);
        }
    }
}