using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Team.Rehab.WebApi.ActionFilters;

namespace Team.Rehab.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi123",
            //    routeTemplate: "api/{controller}/{Action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Filters.Add(new LoggingFilterAttribute());
            config.Filters.Add(new GlobalExceptionAttribute());
        }
    }
}
