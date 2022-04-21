using System;
using System.Web.Http;

namespace hello_world_aspnet_framework_4_6_1.Controllers
{
    public class HealthController : ApiController
    {
        int timeToLive = 60;
        int timeToHealthy = 30;

        [HttpGet()]
        [Route("timeStarted")]
        public string TimeStarted()
        {
            return WebApiConfig.TimeStarted.ToShortTimeString();
        }

        [HttpGet()]
        [Route("alive")]
        public IHttpActionResult Alive()
        {
            return isAlive() ? (IHttpActionResult)Ok() : InternalServerError();
        }

        [HttpGet()]
        [Route("healthy")]
        public IHttpActionResult Healthy()
        {
            return isHealthy() ? (IHttpActionResult)Ok() : InternalServerError();
        }

        private bool isAlive()
        {
            return DateTime.Now < WebApiConfig.TimeStarted.AddSeconds(timeToLive);
        }        

        private bool isHealthy()
        {
            return DateTime.Now < WebApiConfig.TimeStarted.AddSeconds(timeToHealthy);
        }
    }
}
