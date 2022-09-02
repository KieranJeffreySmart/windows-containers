using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace HelloWorld.AsyncProxy.Framework
{
    public class HelloWorldController : ApiController
    {
        private readonly ILogger<HelloWorldController> logger;

        public HelloWorldController()
        {
            logger = new LoggerFactory().CreateLogger<HelloWorldController>();
        }

        [HttpGet()]
        public string HelloWorld()
        {
            logger.LogInformation(1000, $"Get()");
            return "Hello World!!!";
        }

        [HttpGet()]
        [Route("Hello")]
        public string HelloNames([FromUri] string names)
        {
            logger.LogInformation(1000, $"GetSync(names:{names})");
            var hellos = new List<string>();
            foreach (var name in names.Split(','))
            {
                hellos.Add($"Hello {name}!!!");
            }
            logger.LogInformation(1001, $"GetSync(names:{names})");
            return String.Join("\r\n", hellos);
        }

        [HttpGet()]
        [Route("HelloEx")]
        public string ExceptionalHelloWorld()
        {
            throw new Exception("An Exceptional Hello World!!!");
        }
    }
}