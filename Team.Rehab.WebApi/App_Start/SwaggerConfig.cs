using System.Web.Http;
using WebActivatorEx;
using Team.Rehab.WebApi;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Team.Rehab.WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
   .EnableSwagger(c => c.SingleApiVersion("v1", "Team.Rehab.WebApi"))
   .EnableSwaggerUi();
        }
        }
    }

