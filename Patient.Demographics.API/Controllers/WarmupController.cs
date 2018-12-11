using System.Web.Http;
using Patient.Demographics.API.Attributes;

namespace Patient.Demographics.API.Controllers
{
    [RoutePrefix("v1.0/warmup")]
    public class WarmupController : BaseApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}