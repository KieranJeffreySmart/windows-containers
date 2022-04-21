using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace hello_world_aspnet_framework_4_6_1
{
    public static class WebApiConfig
    {
        public static DateTime TimeStarted = DateTime.Now;
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}"
            );
        }
    }
}
