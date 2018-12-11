using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Patient.Demographics.API
{
    public class NoContentResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;

        public NoContentResult( HttpRequestMessage request)
        {
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                RequestMessage = _request
            };
           
            return Task.FromResult(response);
        }
    }
}